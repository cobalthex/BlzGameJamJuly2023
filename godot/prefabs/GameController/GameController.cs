using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameController : Node
{
	public Passenger[] AvailablePassengers { get; private set; } = Array.Empty<Passenger>();
	private Turtle turtleInstance;
	private Timer gameTimer;
	
	[Export]
	public double Score { get; private set; } = 0;

	public override void _Process(double delta)
	{
		// Increment score every second
		Score += delta;
		EventManager.Fire(new ScoreChangedEvent(Score));
	}
	
	public override void _Ready()
	{
		GD.Print("Starting");
		
		// Set Timer
		if (gameTimer == null )
		{
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
				turtleInstance.Transport.PassengerAdded += ManageAvailablePassengers;
				turtleInstance.Transport.PassengerDelivered += AddTime;
				turtleInstance.Transport.PassengerDelivered += ManageAvailablePassengers;
			}
		}

		// Spawn initial Passengers in the world
	}

	private void EndGame()
	{
		GD.Print("Game Over");
		GetTree().Quit();
	}

	// Add 60" to the game time when a passenger is delivered
	// Should be called when a signal is emitted from Turtle.Transport indicating a Passenger was delivered
	private void AddTime()
	{
		gameTimer.WaitTime += Globals.c_baseGameDuration;
		EventManager.Fire(new TimerChangedEvent(gameTimer.WaitTime));
	}

	private void ManageAvailablePassengers()
	{
		GD.Print("We can get some more passengers");
		// if there are less than 4 passenger in availablePassengers, spawn some more in the world
		while (AvailablePassengers.Length > Globals.c_maxPassengers)
		{
			AvailablePassengers.Append(new Passenger());
		}
	}
}
