using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
	private List<Passenger> availablePassengers = new List<Passenger>();
	private Turtle turtleInstance;
	private Timer gameTimer;
	
	[Export]
	public float Score { get; private set; } = 0;

	public override void _Process(double delta)
	{
		// Increment score every second
		Score += 1f;
	}
	
	public override void _Ready()
	{
		GD.Print("Starting");
		
		// Set Timer
		if (gameTimer == null ) {
			var timer = GetParent().GetNodeOrNull<GameTimer>("GameTimer");
			if (timer != null)
			{
				gameTimer = timer;
				gameTimer.Timeout += EndGame;
			}
		}

		// Set Turtle
		if (turtleInstance == null)
		{
			var turtle = GetParent().GetNodeOrNull<Turtle>("Turtle");
			if (turtle != null)
			{
				GD.Print("Turtle Set");
				turtleInstance = turtle;
				turtleInstance.Transport.IsFull += ManageAvailablePassengers;
				turtleInstance.Transport.TransportPassengerDelivered += PassengerDelivered;
			}
		}
	}

	private void EndGame()
	{
		GD.Print("Game Over");
		GetTree().Quit();
	}

	// Add 60" to the game time when a passenger is delivered
	// Should be called when a signal is emitted from Turtle.Transport indicating a Passenger was delivered
	private void PassengerDelivered()
	{
		gameTimer.WaitTime += Globals.c_baseGameDuration;
	}

	private void ManageAvailablePassengers(bool full)
	{
		if (full)
		{
			GD.Print("Turtle is full");
			// remove available passengers from world
		} else
		{
			GD.Print("We can get some more passengers");
			// add some passengers to pick up
		}
	}
}
