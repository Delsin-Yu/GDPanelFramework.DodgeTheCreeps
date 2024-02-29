using Godot;

namespace GDPanelFramework.DodgeTheCreeps;

[GlobalClass]
public partial class SceneObjectsModel : Node
{
    [Export] private Node _mobContainer;
    [Export] private Node _playerContainer;
    [Export] private PathFollow2D _mobSpawnLocation;
    [Export] private Marker2D _playerSpawn;

    public void Deconstruct(out Node mobContainer, out Node playerContainer, out PathFollow2D mobSpawnLocation, out Marker2D playerSpawn)
    {
        mobContainer = _mobContainer;
        playerContainer = _playerContainer;
        mobSpawnLocation = _mobSpawnLocation;
        playerSpawn = _playerSpawn;
    }
}