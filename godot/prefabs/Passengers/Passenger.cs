using Godot;
using System.Diagnostics;

#nullable enable

public partial class Passenger : Area3D
{
	[Export]
	public bool IsDelivered { get; set; } = false;

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

	// Call when Passenger delivered to emit delivery signal and update scoring
	public void SetDelivered(bool value)
	{
		if (IsDelivered != value)
		{
			IsDelivered = value;
			if (IsDelivered)
			{
				EmitSignal(nameof(PassengerDelivered));
			}
		}
	}
}
