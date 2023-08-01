using Godot;
using System.Diagnostics;

#nullable enable

public partial class Passenger : Area3D
{
    [Export]
    public float MaxDeliveryScore { get; set; }

    /// <summary>
    /// This is the time to get half the MaxDeliveryScore, double this time = 0 points
    /// </summary>
    [Export]
    public float DeliveryParTimeSeconds { get; set; }

    [Export]
    public Target Target { get; set; }

    [Signal]
    public delegate void PassengerDeliveredEventHandler();

    public Passenger()
    {
        BodyEntered += Passenger_BodyEntered;
    }

    private void Passenger_BodyEntered(Node3D body)
    {
        if (body is Turtle t)
        {
            t.Transport.TryAddPassenger(this);
        }
    }
}
