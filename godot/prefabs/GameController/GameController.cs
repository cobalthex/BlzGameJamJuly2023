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

		if (!turtleInstance.Transport.IsFull())
		{
			// Make sure there are available passengers
				
		} else {
			// No available passengers
		}
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
			}
		}
	}

	private void EndGame()
	{
		GD.Print("Game Over");
		GetTree().Quit();
	}

	// Add 60" to the game time when a passenger is delivered
	// Should be called when a signal is emitted from any Passenger indicating they were delivered
	private void PassengerDelivered()
	{
		gameTimer.WaitTime += Globals.c_baseGameDuration;
	}
}
