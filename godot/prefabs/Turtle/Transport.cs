using Godot;
using System;

#nullable enable

public partial class Transport : Node3D
{
    public struct Seat
    {
        public Transform3D location;
        public Passenger? passenger;
    }

    public Seat[] Seats { get; private set; } = Array.Empty<Seat>();

    // public override 
}
