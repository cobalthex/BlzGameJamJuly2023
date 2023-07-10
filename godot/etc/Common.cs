using Godot;
using System;

public partial class Common : Node
{
    Label debugText;
    double? m_unpausedTimeScale = null; // if not null, the game is paused

    public override void _Ready()
    {
        debugText = FindChild("debugText") as Label;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key &&
            key.Pressed)
        {
            if (key.Keycode == Key.Q &&
                key.CtrlPressed)
            {
                GetTree().Quit();
            }
            else if (key.Keycode == Key.Escape)
            {
                if (m_unpausedTimeScale.HasValue)
                {
                    Engine.TimeScale = m_unpausedTimeScale.Value;
                    m_unpausedTimeScale = null;
                }
                else
                {
                    m_unpausedTimeScale = Engine.TimeScale;
                    Engine.TimeScale = 0;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (debugText != null)
        {
            debugText.Text = (m_unpausedTimeScale.HasValue ? "Paused" : "");
        }
    }
}
