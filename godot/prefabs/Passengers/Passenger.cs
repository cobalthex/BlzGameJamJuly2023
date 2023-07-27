using Godot;
using System.Diagnostics;

#nullable enable

public partial class Passenger : Area3D
{
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
