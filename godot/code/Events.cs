/// <summary>
/// Events for EventManager
/// </summary>

/*
    // Example Event
    public class ExampleEvent : TEventBase
    {
        public GameObject ExamplePayload { get; set; }
        public int ExampleValue { get; set; }
        public ExampleEvent(GameObject examplePayload, int value)
        {
            ExamplePayload = examplePayload;
            ExampleValue = value;
        }
    }

    // Managing the Listener
    EventManager.AddListener<TopScoreRecievedEvent>(OnTopScoreRecievedEventHandler);
    EventManager.RemoveListener<TopScoreRecievedEvent>();

*/

using System;

public class TimerChangedEvent : TEventBase
{
    public int Time { get; set; }

    public TimerChangedEvent(double timer)
    {
        Time = Convert.ToInt32(timer);
    }
}

public class ScoreChangedEvent : TEventBase
{
    public int Score { get; set; }

    public ScoreChangedEvent(double timer)
    {
        Score = Convert.ToInt32(timer);
    }
}

public class PassengerSpawnedEvent : TEventBase
{
    public Passenger Passenger { get; set; }

    public PassengerSpawnedEvent(Passenger passenger)
    {
        Passenger = passenger;
    }
}
