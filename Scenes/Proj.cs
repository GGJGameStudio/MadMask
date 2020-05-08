using Godot;
using System;

public class Proj : Area2D
{
    private float speed = 200;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * speed * delta;
    }
}
