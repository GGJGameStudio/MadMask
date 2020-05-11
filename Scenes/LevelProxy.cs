using Godot;
using System;

public class LevelProxy : Node
{
    public override void _Ready()
    {
        var level = this.LoadLevel();

        this.ConnectToTeleport(level);
    }

    private Node LoadLevel()
    {
        var levelFactory = GD.Load(Game.GetCurrentLevelPath()) as PackedScene;
        var level = levelFactory.Instance();

        this.AddChild(level);

        return level;
    }

    private void ConnectToTeleport(Node level)
    {
        var teleport = level.GetNode<Teleport>("Teleport");

        if (teleport == null)
        {
            throw new Exception("No teleport found within the level. The node must be named 'Teleport' and located right under the root node of the scene");
        }

        teleport.Connect(nameof(Teleport.OnMainCharacterTeleportation), this, nameof(this.OnTeleportation));
    }

    private void OnTeleportation()
    {
        Game.GetBackToLobby(this.GetTree());
    }
}
