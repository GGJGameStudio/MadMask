using Godot;
using System;

public class LevelProxy : Node
{
    public override void _Ready()
    {
        var level = this.LoadLevel();

        this.ConnectToTeleport(level);
        this.SetBackgroundOnCamera(level);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_cancel"))
        {
            Game.GetBackToLobby(this.GetTree());
        }
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

    private void SetBackgroundOnCamera(Node level)
    {
        var background = this.GetNode("Background");

        this.RemoveChild(background);
        level.GetNode("MainCharacter/Camera2D").AddChild(background);
    }

    private void OnTeleportation()
    {
        Game.GetBackToLobby(this.GetTree());
    }
}
