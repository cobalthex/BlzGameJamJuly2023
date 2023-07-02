using Godot;
using System;

public partial class Common : Node
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key &&
            key.Keycode == Key.Q &&
            key.CtrlPressed)
        {
            GetTree().Quit();
        }
    }
}
