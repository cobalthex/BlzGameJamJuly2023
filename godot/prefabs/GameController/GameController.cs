using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
	private const double COUNTDOWN_DURATION = 60f; // The countdown duration in seconds
	private double timeRemaining = COUNTDOWN_DURATION;
	private List<Passenger> availablePassengers = new List<Passenger>();
	private Turtle turtleInstance;

	public override void _Process(double delta)
	{
		timeRemaining -= delta;

		if (timeRemaining <= 0)
		{
			EndGame();
		}
		
		// Get the Turtle instance when it's spawned or check if it's full
		if (turtleInstance != null) {
			if (!turtleInstance.Transport.IsFull())
			{
				// Make sure there are available passengers
				
			} else {
				// No available passengers
			}
		} else {
			var turtle = GetParent().GetNodeOrNull<Turtle>("Turtle");
			if (turtle != null)
			{
				turtleInstance = turtle;
			}
		}
	}
	
	public override void _Ready()
	{
		GD.Print("Starting");
	}

	private void EndGame()
	{
		GD.Print("Game Over");
		GetTree().Quit();
	}
	
	public void AddTime(double additionalTime)
	{
		timeRemaining += additionalTime;
	}
}
