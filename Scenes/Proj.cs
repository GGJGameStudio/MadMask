using Godot;
using System;

public class Proj : Area2D
{
    private float speed = 200;

    private bool slow = false;

    private float speedSlow = 0.5f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * (slow ? speed * speedSlow : speed) * delta;
    }

    public void SetSlow(bool slow){
        this.slow = slow;
    }

    public bool IsDirectionRight(){
        return Rotation == 0;
    }
}
