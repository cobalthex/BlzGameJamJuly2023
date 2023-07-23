using Godot;

#nullable enable

public partial class Turtle : CharacterBody3D
{
	public const float c_seaWaterDensity = 1050; // Kg/m^3
	public const float c_turtleDragCoefficient = 0.15f;
	public const ulong c_backupMillisecondsDelay = 200;

    [Export]
    public float TurtleMassKg { get; set; } = 150;

    [Export]
    public float TurtleFrontalAreaMSq { get; set; } = 0.25f; // wild guess

    [Export]
	public float ForwardMoveForceNewtons { get; set; } = 1000;

    [Export]
    public float ReverseMoveForceNewtons { get; set; } = 200;

    [Export]
    public float BrakeForceNewtons { get; set; } = 1400;

    [Export]
	public float MaxPitchDegreesPerSec { get; set; } = 50; // cache radians?

	[Export]
	public float MaxYawDegreesPerSec { get; set; } = 50; // cache radians?

	[Export]
	public bool InvertPitchControls { get; set; } = false;

	Vector3 turnPower = Vector3.Zero; // yaw pitch roll
	float m_desiredBankAngle = 0;
	float m_currentBankAngle = 0;
    float m_forwardSpeed = 0;

    Label? m_speedoLabel;

	ulong m_canBackupTime;

	public override void _Ready()
	{
		m_speedoLabel = FindChild("speedo") as Label;
	}

	public override void _Process(double delta)
    {
		Vector3 newRotation = Vector3.Zero;

		float movementDir = Mathf.Sign(m_forwardSpeed);

		turnPower.X = Input.GetActionStrength("YawLeft");
		turnPower.X -= Input.GetActionStrength("YawRight");
		turnPower.X *= movementDir;
        newRotation.Y = (float)(turnPower.X * delta);

		// flip this to invert
        turnPower.Y = -Input.GetActionStrength("PitchUp");
        turnPower.Y += Input.GetActionStrength("PitchDown");
        newRotation.X = (float)(turnPower.Y * (InvertPitchControls ? -1 : 1) * delta);

		newRotation += Rotation;
        // make the turtle lean into a turn based on their speed
        {
			float relSpeed = m_forwardSpeed / 8; // todo, use proper bank angle formula
            float turnSign = Mathf.Sign(Mathf.Floor(turnPower.X * 100)); // round to .01 to add deadzone, todo: scale by dot product of inv Vector.Right
            m_desiredBankAngle = -turnSign * Mathf.Lerp(0, Mathf.Pi * 0.5f, Mathf.Min(1, turnSign * turnPower.X * relSpeed));

			if (turnSign != 0)
			{
				newRotation.Z = Mathf.Lerp(newRotation.Z, m_desiredBankAngle, 0.2f); // todo: do this properly, not with lerp
			}
			else
			{
                newRotation.Z = Mathf.Lerp(newRotation.Z, m_desiredBankAngle, 0.05f);
            }
        }

        turnPower = turnPower.Lerp(Vector3.Zero, 0.2f); // todo: use Ease
		Rotation = newRotation;

		float acceleratorForce = 0;
		float accelStrength = Input.GetActionStrength("Accelerate");
		if (accelStrength > 0)
		{
			acceleratorForce = accelStrength * ForwardMoveForceNewtons;
		}

		float decelStrength = Input.GetActionStrength("Decelerate");
		if (decelStrength > 0)
		{
			// braking
			if (m_forwardSpeed > 0)
			{
				acceleratorForce = decelStrength * -BrakeForceNewtons;
				m_canBackupTime = Time.GetTicksMsec() + c_backupMillisecondsDelay;
			}
			else if (Time.GetTicksMsec() >= m_canBackupTime)
			{
				acceleratorForce = decelStrength * -ReverseMoveForceNewtons;
			}
		}

		var forwardDir = GlobalTransform.Basis.Z;

		float dragForce = 0.5f * c_seaWaterDensity * (movementDir * m_forwardSpeed * m_forwardSpeed) * c_turtleDragCoefficient * TurtleFrontalAreaMSq;

		float acceleration = (acceleratorForce - dragForce) / TurtleMassKg;
        m_forwardSpeed += (float)(acceleration * delta);

		// movement always follows forward direction
		// todo: enumerate all collisions
		var motion = m_forwardSpeed * forwardDir * Globals.c_unitsToMeters * (float)delta;
		if (TestMove(GlobalTransform, motion, m_move))
		{
			GlobalPosition += m_move.GetTravel();
			// todo: slide
			var plane = new Plane(m_move.GetNormal(), m_move.GetPosition());
			GlobalPosition += plane.Project((0.1f * forwardDir).Lerp(motion, 0.8f)); // use friction value
		}
		else
		{
			GlobalPosition += motion;
		}

        // TODO: ocean currents

        if (m_speedoLabel != null)
		{
			m_speedoLabel.Text = 
				$"Euler: {RotationDegrees.ToString("N1")}rad" +
				$"\nForward: {forwardDir.ToString("N2")}" +
				$"\nacceleration: {acceleration:N2}m/s^2" +
				$"\nforward speed: {m_forwardSpeed:N2}m/s";
		}
	}
    KinematicCollision3D m_move = new();
}
