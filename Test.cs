using Godot;
using System;

public class Test : TileMap
{
    private MainCharacter mainCharacter;
    private Vector2 initialPosition;

    public override void _Ready()
    {
        this.mainCharacter = this.GetNode<MainCharacter>("MainCharacter");
        this.initialPosition = this.mainCharacter.Position;
    }

    public override void _Process(float delta)
    {
        if (this.mainCharacter.Position.x < -100 || this.mainCharacter.Position.x > 2400
            || this.mainCharacter.Position.y > 700)
        {
            this.mainCharacter.Position = initialPosition;
        }
    }
}
