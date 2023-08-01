using Godot;

public partial class Target : Area3D
{
    public override void _Ready()
    {
        BodyEntered += Target_BodyEntered;
    }

    private void Target_BodyEntered(Node3D body)
    {
        if (body is Turtle t)
        {
            t.Transport.TryDeliverPassengers(this);
        }
    }
}
