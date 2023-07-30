using Godot;

public partial class GameTimer : Timer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.WaitTime = Globals.c_baseGameDuration;
        this.OneShot = true;
        this.Start();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        TimerRoutine();
    }

    private async void TimerRoutine()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        EventManager.Fire(new TimerChangedEvent(this.TimeLeft));
    }
}
