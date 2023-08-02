using Godot;

public partial class PassengerSpawner : CsgBox3D
{
    [Export]
    public PackedScene[] m_passengerPackedScene;

    [Export]
    public Target[] AvailableTargets { get; set; }

    private Vector3 spawnExtent;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (m_passengerPackedScene == null || m_passengerPackedScene.Length == 0)
        {
            GD.PrintErr("No Passengers on spawner ", Name);
            return;
        }
        if (AvailableTargets == null)
        {
            GD.PrintErr("No available targets for passengers");
            return;
        }

        var passengerScene = m_passengerPackedScene[GD.RandRange(0, m_passengerPackedScene.Length-1)];
        var passengerResource = GD.Load<PackedScene>(passengerScene.ResourcePath);
        Passenger passenger = passengerResource.Instantiate<Passenger>();

        // Randomize position
        spawnExtent = Scale * 0.5f;
        Vector3 spawnPosition = Position + new Vector3((float)GD.RandRange(-spawnExtent.X, spawnExtent.X), 0f, (float)GD.RandRange(-spawnExtent.Z, spawnExtent.Z));
        //GD.PrintS("Passenger spawned at", spawnPosition.ToString());

        passenger.Target = AvailableTargets[GD.RandRange(0, AvailableTargets.Length - 1)];

        AddChild(passenger);
        passenger.GlobalPosition = spawnPosition;
        passenger.GlobalScale(new Vector3(1f/Scale.X, 1f/Scale.Y, 1f/Scale.Z));
        passenger.GlobalRotate(Vector3.Up, (float)GD.RandRange(-180f, 180f));

        // Fire message that passenger got created
        EventManager.Fire(new PassengerSpawnedEvent(passenger));

        // Hide Geo
        var parent = passenger.GetParent();
    }
}
