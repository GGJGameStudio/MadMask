using Godot;
using System;

public class CharacterHitbox : Area2D
{
    
    MainCharacter character;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, nameof(_onSpikeCollide));

        character = GetParent() as MainCharacter;
    }

    public void _onSpikeCollide(Area2D area)
    {
        if (!character.IsDashing() && area is Spike){
            GD.Print("dead");
        }
    }
}
