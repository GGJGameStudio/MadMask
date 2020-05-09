using Godot;
using System;

public class Slow : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, nameof(_onSlowEnter));
        this.Connect("area_exited", this, nameof(_onSlowExit));
    }

    
    public void _onSlowEnter(Area2D area)
    {
        if (area.GetType() == typeof(Proj)){
            var proj = area as Proj;
            proj.SetSlow(true);
        }
    }

    public void _onSlowExit(Area2D area)
    {
        if (area.GetType() == typeof(Proj)){
            var proj = area as Proj;
            proj.SetSlow(false);
        }
    }
}
