using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class GameController : Node
{
    [Export]
    public float DeliveryTimeAddSeconds { get; set; } = 30;

    public List<Passenger> WorldPassengers = new List<Passenger>();
    public Passenger[] AvailablePassengers { get; private set; } = Array.Empty<Passenger>();

    private Turtle m_turtleInstance;
    private Timer m_gameTimer;

    public enum GameState
    {
        NotStarted,
        Running,
        GameOver,
    }

    public GameState State { get; set; } = GameState.NotStarted;

    public int Score => Mathf.FloorToInt(m_score);
    private float m_score;

    public override void _EnterTree()
    {
        base._EnterTree();
        EventManager.AddListener<PassengerSpawnedEvent>(OnPassengerSpawnedEvent);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.RemoveListener<PassengerSpawnedEvent>(OnPassengerSpawnedEvent);
    }

    public override void _Ready()
    {
        GD.Print("Starting");
        
        // Set Timer
        if (m_gameTimer == null)
        {
            var timer = GetParent().GetNodeOrNull<Timer>("GameTimer");
            if (timer != null)
            {
                m_gameTimer = timer;
                m_gameTimer.Timeout += EndGame;
            }
        }

        // Set Turtle
        if (m_turtleInstance == null)
        {
            var turtle = GetParent().GetNodeOrNull<Turtle>("Turtle");
            if (turtle != null)
            {
                m_turtleInstance = turtle;
                m_turtleInstance.Transport.PassengerDelivered += OnPassengerDelivered;
            }
            else
            {
                GD.PrintErr("Could not find turtle");
            }
        }

        // Spawn initial Passengers in the world

        State = GameState.Running;
    }

    private void EndGame()
    {
        GD.Print("Game Over");
        State = GameState.GameOver;
    }

    private void OnPassengerDelivered(float deliveryScore)
    {
        m_score += deliveryScore;
        m_gameTimer.WaitTime += DeliveryTimeAddSeconds;
    }

    private void OnPassengerSpawnedEvent(PassengerSpawnedEvent evt)
    {
        WorldPassengers.Append(evt.Passenger);
    }
}
