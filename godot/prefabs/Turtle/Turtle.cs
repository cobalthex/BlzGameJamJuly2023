using Godot;
using System;

public partial class Turtle : Node3D
{
	// todo: use curves
	[Export]
	public Curve Acceleration { get; set; }
	[Export]
	public float MaxForwardSpeed { get; set; } = 50;

    [Export]
    public float MaxReverseSpeed { get; set; } = 4;


    [Export]
	public float MaxBackwardMetersPerSec { get; set; } = 3;

	[Export]
	public float MaxPitchDegreesPerSec { get; set; } = 20; // cache radians?

    [Export]
    public float MaxYawDegreesPerSec { get; set; } = 20; // cache radians?

    float m_morwardSpeed = 0;

	Label m_speedoLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_speedoLabel = FindChild("speedo") as Label;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float pitchScale = 0;
		if (Input.IsActionPressed("PitchDown"))
		{
			pitchScale = 1;
		}
		if (Input.IsActionPressed("PitchUp"))
		{
			pitchScale = -1;
		}
		if (pitchScale != 0)
		{
			RotateX(pitchScale * (float)delta * Mathf.DegToRad(MaxPitchDegreesPerSec));
        }

		float yawScale = 0;
        if (Input.IsActionPressed("YawLeft"))
        {
            yawScale = 1;
        }
        if (Input.IsActionPressed("YawRight"))
        {
            yawScale = -1;
        }
		if (yawScale != 0)
		{
			RotateY(yawScale * (float)delta * Mathf.DegToRad(MaxYawDegreesPerSec));
		}
		// TODO: rotations don't work

		float accelStrength = Input.GetActionStrength("Accelerate");
		if (accelStrength > 0)
		{
			float forwardAccel = Acceleration.SampleBaked(m_morwardSpeed / MaxForwardSpeed);
			m_morwardSpeed += accelStrength * forwardAccel;
		}

		float decelStrength = Input.GetActionStrength("Decelerate");
		if (decelStrength > 0)
		{
			// braking
			if (m_morwardSpeed > 0)
			{
				m_morwardSpeed = Mathf.Lerp(m_morwardSpeed, 0, 0.1f); // TEST
			}
			else
			{
				float reverseAccel = Acceleration.SampleBaked(-m_morwardSpeed / MaxReverseSpeed);
                m_morwardSpeed -= accelStrength * reverseAccel;
				GD.PrintS(reverseAccel, m_morwardSpeed);
            }
		}

        Position += Basis.Z * (float)(m_morwardSpeed * delta);

		m_speedoLabel.Text = $"{m_morwardSpeed:N2}m/s";
    }
}
