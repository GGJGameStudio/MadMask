using Godot;
using System;

public class Canon : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private PackedScene proj = (PackedScene) GD.Load("res://scenes/Proj.tscn");

    [Export]
    public float rotation;

    [Export]
    public float rate = 2f;

    [Export]
    public float start = 1f;

    private float timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = start;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        timer -= delta;

        if (timer < 0){
            var proj_instance = (Node2D) proj.Instance();
            proj_instance.Rotation = Mathf.Deg2Rad(rotation);
            AddChild(proj_instance);

            timer += 1/rate;
        }
    }
}
