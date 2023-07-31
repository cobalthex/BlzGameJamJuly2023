using Godot;
using System;

public partial class Swim : PathFollow3D
{
    [Export]
    public float SpeedMetersPerSecond { get; set; } = 10;

    [Export]
    public float MaxTurnRadiansPerSecond { get; set; } = Mathf.Pi;

    [Export]
    public bool WillFlee { get; set; } = true;

    public enum SwimState
    {
        NagivatingToPath,
        FollowingPath,
        Fleeing,
    }

    public SwimState State { get; private set; }

    ulong m_navigateTimeEndMillisec;
    Quaternion m_navigateQuat;

    Label3D m_debugText;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UseModelFront = true;
        Loop = true;
        CubicInterp = true;

        State = SwimState.FollowingPath;
        Progress = 0;

        m_debugText = (Label3D)FindChild("debugText");

        ((Area3D)FindChild("FieldOfView")).BodyEntered += FieldOfViewEntered;
    }

    private void FieldOfViewEntered(Node3D body)
    {
        if (IsAncestorOf(body))
        {
            return;
        }

        GD.PrintS($"Fleeing from {body.Name}");

        // this shouldn't be necessary
        if (State == SwimState.Fleeing)
        {
            return;
        }

        // TODO: use collision layers/masks to restrict this, only swim away from some types of fish
        State = SwimState.Fleeing;
        m_navigateTimeEndMillisec = Time.GetTicksMsec() + (ulong)Random.Shared.NextInt64(1000, 3000);
        var desiredDirection = (GlobalPosition - body.GlobalPosition).Normalized();
        MoveTowards(desiredDirection);
    }

    void MoveTowards(Vector3 direction)
    {
        var refVec = Vector3.Up;
        var axis = refVec.Cross(direction).Normalized();
        var angle = Mathf.Acos(refVec.Dot(direction));
        m_navigateQuat = new Quaternion(axis, angle);
    }

    void NavigateToPath()
    {
        State = SwimState.NagivatingToPath;
        m_navigateTimeEndMillisec = 0;

        var path = GetParent<Path3D>();

        //var desiredOffset = path.Curve.GetClosestOffset(Position); // ideally this would find an intersection between a forward ray and the path
        //var transform = path.Curve.SampleBakedWithRotation(desiredOffset, true);


        Progress = path.Curve.GetClosestOffset(Position);
        MoveTowards(path.Curve.SampleBaked(Progress) - Position);
    }

    private readonly Quaternion c_qUp = new Quaternion(Vector3.Up, Mathf.Pi / 2);

    public override void _Process(double deltaTime)
    {
        if (State == SwimState.FollowingPath)
        {
            Progress += SpeedMetersPerSecond * (float)deltaTime;
        }
        else
        {
            if (Time.GetTicksMsec() >= m_navigateTimeEndMillisec)
            {
                if (State == SwimState.Fleeing)
                {
                    NavigateToPath();
                }
                else // this is janky
                {
                    State = SwimState.FollowingPath;
                }
            }
            else
            {
                Quaternion = Quaternion.Slerp(m_navigateQuat, MaxTurnRadiansPerSecond * (float)deltaTime);
                Position += Basis.Z * SpeedMetersPerSecond * (float)deltaTime;
            }
        }
        Quaternion = Quaternion.Slerp(c_qUp, 0.2f); // test

        m_debugText.Text = $"{State} {m_navigateQuat} {m_navigateTimeEndMillisec}";
    }
}
