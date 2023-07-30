using Godot;

public partial class UIManager : Control
{
    private Label? m_timerValue;
    private Label? m_scoreValue;

    public override void _Ready()
    {
        m_timerValue = GetNode<Label>("TimerRect/TimerValue");
        m_scoreValue = GetNode<Label>("ScoreRect/ScoreValue");
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        EventManager.AddListener<TimerChangedEvent>(OnTimerChangedEvent);
        EventManager.AddListener<ScoreChangedEvent>(OnScoreChangedEvent);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.RemoveListener<TimerChangedEvent>(OnTimerChangedEvent);
        EventManager.RemoveListener<ScoreChangedEvent>(OnScoreChangedEvent);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnTimerChangedEvent(TimerChangedEvent evt)
    {
        m_timerValue.Text = evt.Time > 0 ? evt.Time.ToString() : "0";
    }
    
    private void OnScoreChangedEvent(ScoreChangedEvent evt)
    {
        m_scoreValue.Text = evt.Score.ToString();
    }
}
