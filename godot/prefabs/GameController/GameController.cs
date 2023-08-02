using Godot;

public partial class GameController : Node
{
    [Export]
    public float DeliveryTimeAddSeconds { get; set; } = 30;

    [Export]
    public Turtle Turtle { get; set; }

    private Timer m_gameTimer;

    private Label m_timerLabel;
    private Label m_scoreLabel;

    public enum GameState
    {
        NotStarted,
        Running,
        GameOver,
    }

    public GameState State { get; set; } = GameState.NotStarted;

    public int Score => Mathf.FloorToInt(m_score);
    private float m_score;

    public override void _Ready()
    {
        GD.Print("Starting");

        Turtle.Transport.PassengerDelivered += OnPassengerDelivered;

        m_gameTimer = GetNode<Timer>("GameTimer");
        m_gameTimer.Timeout += EndGame;

        m_timerLabel = GetNode<Label>("Hud/TimerRect/TimerValue");
        m_scoreLabel = GetNode<Label>("Hud/ScoreRect/ScoreValue");

        // Spawn initial Passengers in the world

        State = GameState.Running;
    }

    public override void _Process(double delta)
    {
        m_timerLabel.Text = $"{m_gameTimer.TimeLeft:N1}s";
        m_scoreLabel.Text = Score.ToString();
    }

    private void EndGame()
    {
        GD.Print("Game Over");
        State = GameState.GameOver;
    }

    private void OnPassengerDelivered(float deliveryScore)
    {
        m_score += deliveryScore;
        m_gameTimer.Start(m_gameTimer.TimeLeft + DeliveryTimeAddSeconds);
    }
}
