using Godot;
using System.Collections.Generic;

public partial class Target : Area3D
{
    [Export]
    public float MaxDeliverySpeed { get; set; } = 0.01f;

    [Export]
    public uint DeliveryHoldTime { get; set; } = 500;

    private Turtle m_enteredTurtle; // support more than one turtle?
    private ulong m_canDeliverTimeMillisec;

    public override void _Ready()
    {
        BodyEntered += (body) =>
        {
            if (m_enteredTurtle == null &&
                body is Turtle t)
            {
                m_enteredTurtle = t;
                m_canDeliverTimeMillisec = Time.GetTicksMsec() + DeliveryHoldTime;
            }
        };
        BodyExited += (body) =>
        {
            if (body == m_enteredTurtle)
            {
                m_enteredTurtle = null;
            }
        };
    }

    public override void _Process(double delta)
    {
        if (m_enteredTurtle != null)
        {
            if (Mathf.Abs(m_enteredTurtle.ForwardSpeed) > MaxDeliverySpeed)
            {
                m_canDeliverTimeMillisec = Time.GetTicksMsec() + DeliveryHoldTime;
            }
            else if (Time.GetTicksMsec() >= m_canDeliverTimeMillisec)
            {
                m_enteredTurtle.Transport.TryDeliverPassengers(this);
            }
        }
    }
}
