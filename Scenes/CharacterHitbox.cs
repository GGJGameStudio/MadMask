using Godot;
using System;

public class CharacterHitbox : Area2D
{
    
    MainCharacter character;

    private float hbumpForce = 250f;
    private float vbumpForce = 400f;
    private float bumpDuration = 0.3f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, nameof(_onAreaEnter));

        character = GetParent() as MainCharacter;
    }

    public void _onAreaEnter(Area2D area)
    {
        if (area is Spike){
            character.Restart();
        }

        if (area is Proj proj){
            character.Bump(new Vector2((proj.IsDirectionRight() ? 1 : -1) * hbumpForce,-vbumpForce), bumpDuration);
            proj.QueueFree();
        }
    }
}
