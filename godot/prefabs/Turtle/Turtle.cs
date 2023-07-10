using Godot;

#nullable enable

public partial class Turtle : Node3D
{
	public const float c_seaWaterDensity = 1050; // Kg/m^3
	public const float c_turtleDragCoefficient = 0.15f;

    [Export]
    public float TurtleMassKg { get; set; } = 150;

    [Export]
    public float TurtleFrontalAreaMSq { get; set; } = 0.25f; // wild guess

    [Export]
	public float ForwardMoveForceNewtons { get; set; } = 150;

	[Export]
	public float MaxPitchDegreesPerSec { get; set; } = 50; // cache radians?

	[Export]
	public float MaxYawDegreesPerSec { get; set; } = 50; // cache radians?

	Vector3 turnPower = Vector3.Zero; // yaw pitch roll
	float m_desiredBankAngle = 0;
    float m_forwardSpeed = 0;

    Label? m_speedoLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_speedoLabel = FindChild("speedo") as Label;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 newRotation = Vector3.Zero;

		// todo: smooth turns from bank to bank

		if (Input.IsActionPressed("YawLeft"))
		{
			turnPower.X = 1;
		}
		if (Input.IsActionPressed("YawRight"))
		{
			turnPower.X = -1;
		}
        newRotation.Y = (float)(turnPower.X * delta); // todo: should be affected by speed (like a car)

		if (Input.IsActionPressed("PitchDown"))
		{
			turnPower.Y = 1;
		}
		if (Input.IsActionPressed("PitchUp"))
		{
			turnPower.Y = -1;
        }
        newRotation.X = (float)(turnPower.Y * delta);

		newRotation += Rotation;
        // make the turtle lean into a turn based on their speed
        {
			float relSpeed = m_forwardSpeed / 5; // todo, use proper bank angle formula
            float turnSign = Mathf.Sign(turnPower.X);
            m_desiredBankAngle = -turnSign * Mathf.Lerp(0, Mathf.Pi * 0.5f, Mathf.Min(1, turnSign * turnPower.X * relSpeed));
			newRotation.Z = Mathf.Lerp(newRotation.Z, m_desiredBankAngle, 0.2f); // todo: do this properly, not with lerp
            //newRotation.Z = Mathf.Lerp(newRotation.Z, 0, 0.1f); // slerp?
        }

        turnPower = turnPower.Lerp(Vector3.Zero, 0.2f); // todo: use Ease
		Rotation = newRotation;

		float acceleratorForce = 0;
		float accelStrength = Input.GetActionStrength("Accelerate");
		if (accelStrength > 0)
		{
			acceleratorForce = accelStrength * ForwardMoveForceNewtons;
			//float forwardAccel = Acceleration.SampleBaked(accelStrength * (m_forwardSpeed / MaxForwardSpeed));
			//acceleratorPower = accelStrength * forwardAccel;
		}

		/*float decelStrength = Input.GetActionStrength("Decelerate");
		if (decelStrength > 0)
		{
			// braking
			if (acceleratorPower > 0)
			{
				acceleratorPower = Mathf.Lerp(acceleratorPower, 0, 0.1f); // todo: TEST
			}
			else
			{
				float reverseAccel = Acceleration.SampleBaked(-acceleratorPower / MaxReverseSpeed);
				acceleratorPower -= accelStrength * reverseAccel;
			}
		}*/

		var forwardDir = GlobalTransform.Basis.Z;

		float dragForce = 0.5f * c_seaWaterDensity * (m_forwardSpeed * m_forwardSpeed) * c_turtleDragCoefficient * TurtleFrontalAreaMSq;

		float acceleration = (acceleratorForce - dragForce) / TurtleMassKg;

        m_forwardSpeed += (float)(acceleration * delta);
        GlobalPosition += m_forwardSpeed * forwardDir;

		//float relSpeed = m_forwardSpeed / MaxForwardSpeed;
		//m_forwardSpeed *= Mathf.Lerp(1, 1 - Drag, relSpeed);

        // TODO: ocean currents

        if (m_speedoLabel != null)
		{
			m_speedoLabel.Text = 
				$"Euler: {RotationDegrees.ToString("N1")}rad" +
				$"\nForward: {forwardDir.ToString("N2")}" +
				$"\nacceleration: {acceleration:N2}m/s^2" +
				$"\nforward speed: {m_forwardSpeed * Globals.c_unitsToMeters:N2}m/s";
		}
	}
}
