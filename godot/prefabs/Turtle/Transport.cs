using Godot;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable

public partial class Transport : Node3D
{
	public const string c_seatNamePattern = "Seat-*";

	Label? debugText;

	public struct Seat
	{
		public Node3D node;
		public Passenger? passenger;
	}

	public Seat[] Seats { get; private set; } = Array.Empty<Seat>();

	[Signal]
	public delegate void IsFullEventHandler(bool full);

	public override void _Ready()
	{
		Seats = FindChildren(c_seatNamePattern, nameof(Node3D), false)
			.Select(s => new Seat { node = ((Node3D)s), passenger = null })
			.ToArray();
		// sort seats around a circle?

		// TODO: do ^ in child_entered_tree?

		debugText = FindChild("debug") as Label;
	}

	/// <summary>
	/// Try and add a passenger to the transport
	/// </summary>
	/// <returns>True if the passenger was added, false if not (no free seats)</returns>
	/// <remarks>This transport will parent the passenger if added, and handle movement</remarks>
	public bool TryAddPassenger(Passenger passenger)
	{
		// TODO: find the closest seat
		for (int i = 0; i < Seats.Length; ++i)
		{
			if (Seats[i].passenger != null)
			{
				continue;
			}

			Seats[i].passenger = passenger;
			var relTransform = passenger.Transform;
			passenger.GetParent()?.RemoveChild(passenger);
			Seats[i].node.AddChild(passenger);
			passenger.Position = Seats[i].node.Transform.Origin; // todo: this needs rotation+scale

			// todo: passenger needs to swim to desird location (passenger should do that automatically?)

			GD.PrintS("Picked up passenger", passenger);
			return true;
		}

		return false;
	}
	
	// Emit a signal indicating if the Transport has room for more passengers, or not
	public void CheckIfFull()
	{
		for (int i = 0; i < Seats.Length; ++i)
		{
			if (Seats[i].passenger == null)
			{
				EmitSignal(SignalName.IsFull, false);
				return;
			}
		}

		EmitSignal(SignalName.IsFull, true);
		return;
	}

	public override void _Process(double delta)
	{
		if (debugText != null)
		{
			StringBuilder sb = new();
			for (int i = 0; i < Seats.Length; ++i)
			{
				sb.Append(i);
				sb.Append(": ");
				sb.Append(Seats[i].node.Name);
				sb.Append(' ');
				sb.AppendLine(Seats[i].passenger?.Name ?? "(none)");
			}
			debugText.Text = sb.ToString();
		}

		CheckIfFull();
	}
}
