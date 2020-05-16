using Godot;
using System;

public class Platform : KinematicBody2D, Entity
{

    private Movable movable;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        movable = GetNode("Movable") as Movable;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    public Movable GetMovable(){
        return movable;
    }

    public Slowable GetSlowable(){
        return movable.Slowable;
    }
}
