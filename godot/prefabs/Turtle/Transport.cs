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
		public ulong pickupTimeMillisec;
	}

	public Seat[] Seats { get; private set; } = Array.Empty<Seat>();

	[Signal]
	public delegate void PassengerAddedEventHandler();

	[Signal]
	public delegate void PassengerDeliveredEventHandler(float deliveryScore);

	public override void _Ready()
	{
		Seats = FindChildren(c_seatNamePattern, nameof(Node3D), false)
			.Select(s => new Seat { node = ((Node3D)s), passenger = null })
			.ToArray();
		// sort seats around a circle?

		// TODO: do ^ in child_entered_tree?

#if DEBUG
		debugText = FindChild("debug") as Label;
#endif
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

			// connect to PassengerDelivered signal
			passenger.PassengerDelivered += TransportPassengerWasDelivered;

			// todo: passenger needs to swim to desird location (passenger should do that automatically?)

			GD.PrintS("Picked up passenger", passenger);
			return true;
		}
		EmitSignal(SignalName.PassengerAdded);

		// tell target that it's active?

		return false;
	}

	private float CalculateScore(Passenger passenger, float secondsTaken) =>
        Mathf.Clamp(1 - secondsTaken / (passenger.DeliveryParTimeSeconds * 2), 0, 1) * passenger.MaxDeliveryScore;

	public void TryDeliverPassengers(Target target)
	{
		if (target == null)
		{
			return;
		}

		for (int i = 0; i < Seats.Length; ++i)
		{
			var passenger = Seats[i].passenger;
			if (passenger != null &&
				passenger.Target == target)
			{
                // calculate score
                var timeTaken = (Time.GetTicksMsec() - Seats[i].pickupTimeMillisec) / 1000f;
				var deliveryScore = CalculateScore(passenger, timeTaken);

                EmitSignal(nameof(PassengerDelivered), deliveryScore);
                GD.PrintS("Passenger delivered in", timeTaken, "secs,", deliveryScore, "points");
				Seats[i].passenger = null;

				Seats[i].node.RemoveChild(passenger);
				GetTree().QueueDelete(passenger);
			}
		}
	}

	public void TransportPassengerWasDelivered()
	{
		EmitSignal(SignalName.PassengerDelivered);
		return;
	}

	public override void _Process(double delta)
	{
		var now = Time.GetTicksMsec();

		if (debugText != null)
		{
			StringBuilder sb = new();
			for (int i = 0; i < Seats.Length; ++i)
			{
				sb.Append(i);
				sb.Append(": ");
				sb.Append(Seats[i].node.Name);
				sb.Append(' ');
				var passenger = Seats[i].passenger;
				if (passenger != null)
				{
					sb.Append(passenger.Name);
					sb.Append("  ");
					var timeTaken = Time.GetTicksMsec() - Seats[i].pickupTimeMillisec;
					sb.Append(timeTaken / 1000);
					sb.Append('.');
					sb.Append((timeTaken % 1000).ToString("D3"));
					sb.Append(" / ");
                    sb.Append(passenger.DeliveryParTimeSeconds.ToString("N3"));
                    sb.Append(" sec,  ");
					sb.Append(CalculateScore(passenger, timeTaken / 1000f).ToString("N1"));
					sb.Append(" / ");
                    sb.Append(passenger.MaxDeliveryScore.ToString("N1"));
                    sb.AppendLine(" pts");
				}
				else
				{
					sb.AppendLine("(none)");
				}
				
			}
			debugText.Text = sb.ToString();
		}
	}
}
