using Godot;

#nullable enable

public partial class Turtle : CharacterBody3D
{
	public const float c_seaWaterDensity = 1050; // Kg/m^3
	public const float c_turtleDragCoefficient = 0.15f;
	public const ulong c_backupMillisecondsDelay = 300;

	[Export]
	public float TurtleMassKg { get; set; } = 150;

	[Export]
	public float TurtleFrontalAreaMSq { get; set; } = 0.25f; // wild guess

	[Export]
	public float ForwardMoveForceNewtons { get; set; } = 1000;

	[Export]
	public float ReverseMoveForceNewtons { get; set; } = 200;

	[Export]
	public float BrakeForceNewtons { get; set; } = 1000;

	[Export]
	public float MaxYawDegreesPerSec { get; set; } = 50; // cache radians?

	[Export]
	public float MaxPitchDegreesPerSec { get; set; } = 60; // cache radians?

	[Export]
	public bool InvertPitchControls { get; set; } = true;

	[Export]
	public bool InvertJoypadPitchControls { get; set; } = false;

	KinematicCollision3D m_collision = new();
	float m_desiredBankAngleRadians = 0;
	float m_currentBankAngle = 0;
	float m_forwardSpeed = 0;
	ulong m_canBackupTime;
	bool m_didCollide;

	Label? m_debugText;

	// child elements
	public Transport Transport { get; private set; }

	public override void _Ready()
	{
#if DEBUG
		m_debugText = GetNode<Label>("debug");
#endif
		Transport = GetNode<Transport>("model/transport");
	}

	// TODO: fix controller movement
	void HandleMovement(double deltaTimeD)
	{
		float deltaTime = (float)deltaTimeD;
		float movementDir = Mathf.Sign(m_forwardSpeed);

		// keyboard
		Vector3 input = new();
		input.X = Input.GetActionStrength("YawLeft");
		input.X -= Input.GetActionStrength("YawRight");
		input.Y = Input.GetActionStrength("PitchUp");
		input.Y -= Input.GetActionStrength("PitchDown");
		if (InvertPitchControls)
		{
			input.Y *= -1;
		}

		Vector3 joypadInput = new();
		joypadInput.X = Input.GetActionStrength("YawLeftJoypad");
		joypadInput.X -= Input.GetActionStrength("YawRightJoypad");
		joypadInput.Y = Input.GetActionStrength("PitchUpJoypad");
		joypadInput.Y -= Input.GetActionStrength("PitchDownJoypad");
		if (InvertJoypadPitchControls)
		{
			joypadInput.Y *= -1;
		}

		if (!joypadInput.IsZeroApprox()) // per axis?
		{
			input = joypadInput;
		}

		// todo: if did collide, don't continue allowing rotation in that direction

		Vector3 newRotation = Vector3.Zero;
		newRotation.X = input.Y * Mathf.DegToRad(MaxYawDegreesPerSec);

		float turnScale = input.X * (m_forwardSpeed < 0 ? -1 : 1) * Mathf.DegToRad(MaxPitchDegreesPerSec);
		const float c_maxSpeed = 8; // todo: don't hard code this
		float relSpeed = Mathf.Clamp(m_forwardSpeed / c_maxSpeed, -c_maxSpeed, c_maxSpeed);

		// make the turtle lean into a turn based on their speed
		// todo: separate turtle model rotation?
		{
			float maxBankAngleRadians = Mathf.Min(Mathf.Pi * 0.3f, Mathf.DegToRad(MaxPitchDegreesPerSec) * 2);
			float turnSign = Mathf.Sign(Mathf.Floor(input.X * 100)); // round to .01 to add deadzone (avoids issues of being almost but not exactly 0)
																	 // todo: scale by dot product of inv Vector.Right
			m_desiredBankAngleRadians = -turnSign * Mathf.Lerp(0, maxBankAngleRadians, Mathf.Min(1, turnSign * turnScale * relSpeed));

			// this sucks
			const float c_activeBankLerp = Mathf.Pi * 0.5f;
			const float c_passiveBankLerp = c_activeBankLerp / 2;
			float bankLerp = (m_desiredBankAngleRadians == 0 ? c_passiveBankLerp : c_activeBankLerp);
			newRotation.Z = Mathf.MoveToward(Rotation.Z, m_desiredBankAngleRadians, bankLerp * deltaTime); // lerp would be better
		}

		newRotation.Y = movementDir * input.X * Mathf.Lerp(MaxPitchDegreesPerSec, MaxPitchDegreesPerSec / 2, relSpeed) * deltaTime;
		newRotation.X = (newRotation.X * deltaTime) + Rotation.X;
		newRotation.Y = (newRotation.Y * deltaTime) + Rotation.Y;

		newRotation.X = Mathf.Clamp(newRotation.X, -Mathf.Pi * 0.45f, Mathf.Pi * 0.45f);

		Rotation = newRotation;

		float acceleratorForce = 0;
		float accelStrength = Input.GetActionStrength("Accelerate");
		if (accelStrength > 0)
		{
			acceleratorForce = accelStrength * ForwardMoveForceNewtons;
		}

		bool isBraking = false;
		float decelStrength = Input.GetActionStrength("Decelerate");
		if (decelStrength > 0)
		{
			// braking
			if (m_forwardSpeed > 0)
			{
				// todo: brake force should round off to zero as speed approaches zero
				acceleratorForce = decelStrength * -BrakeForceNewtons;
				m_canBackupTime = Time.GetTicksMsec() + c_backupMillisecondsDelay;
				isBraking = true;
			}
			else if (Time.GetTicksMsec() >= m_canBackupTime)
			{
				acceleratorForce = decelStrength * -ReverseMoveForceNewtons;
			}
		}

		var forwardDir = GlobalTransform.Basis.Z;

		float dragForce = 0.5f * c_seaWaterDensity * (movementDir * m_forwardSpeed * m_forwardSpeed) * c_turtleDragCoefficient * TurtleFrontalAreaMSq;

		float acceleration = (acceleratorForce - dragForce) / TurtleMassKg;
		m_forwardSpeed += (float)(acceleration * deltaTime);

		// avoids braking overshoot (force should really be based on speed)
		if (isBraking && Mathf.Abs(m_forwardSpeed) < 0.1f)
		{
			m_forwardSpeed = 0;
		}

		// movement always follows forward direction
		// todo: enumerate all collisions
		var motion = m_forwardSpeed * forwardDir * Globals.c_unitsToMeters * deltaTime;
		if (TestMove(GlobalTransform, motion, m_collision))
		{
			var plane = new Plane(m_collision.GetNormal(), Vector3.Zero);

			var relativeOrientation = forwardDir.Dot(plane.Normal);
			// slide
			if (relativeOrientation > -0.6f)
			{
				// todo: use actual friction forces
				motion *= 0.8f * (1 + relativeOrientation); // reduce speed the more perpendicular to the collider
				motion = plane.Project(motion); // project the motion onto the plane to slide the turtle
			}
			else
			{
				motion = Vector3.Zero;
				//m_forwardSpeed = 0.1f;
			}
			GlobalPosition += m_collision.GetTravel() + motion;
		}
		else
		{
			GlobalPosition += motion;
		}

		// TODO: ocean currents

		if (m_debugText != null)
		{
			m_debugText.Text =
				$"Euler: {RotationDegrees.ToString("N1")}deg"
				+ $"\nForward: {forwardDir.ToString("N2")}"
				+ $"\nAcceleration: {acceleration:N2}m/s^2"
				+ $"\nForward speed: {m_forwardSpeed:N2}m/s"
				+ $"\nBank angle: {m_desiredBankAngleRadians:N2}rad";
		}
	}

	// use _PhysicsProcess?
	public override void _Process(double deltaTime)
	{
		HandleMovement(deltaTime);
	}
}
