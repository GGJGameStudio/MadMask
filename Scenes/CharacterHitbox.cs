using Godot;
using System;

public class CharacterHitbox : Area2D
{
    
    MainCharacter character;

    private float bumpForce = 500f;
    private float bumpDuration = 0.3f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, nameof(_onAreaEnter));

        character = GetParent() as MainCharacter;
    }

    public void _onAreaEnter(Area2D area)
    {
        if (!character.IsDashing()){
            if (area is Spike){
                GD.Print("dead");
            }

            if (area is Proj proj){
                GD.Print("bump");
                character.Bump(new Vector2((proj.IsDirectionRight() ? 1 : -1) * bumpForce,-bumpForce), bumpDuration);
                proj.QueueFree();
            }
        }
        


    }
}
