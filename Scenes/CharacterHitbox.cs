using Godot;
using System;

public class CharacterHitbox : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, nameof(_onSpikeCollide));
    }

    public void _onSpikeCollide(Area2D area)
    {
        if (area is Spike){
            GD.Print("dead");
        }
    }
}
