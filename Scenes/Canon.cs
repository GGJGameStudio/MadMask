using Godot;
using System;

public class Canon : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private PackedScene proj = (PackedScene)GD.Load("res://scenes/Proj.tscn");

    private EntityOrientation orientation;

    [Export]
    public EntityOrientation Orientation
    {
        get => orientation;
        set => this.SetOrientation(value);
    }

    private void SetOrientation(EntityOrientation orientation)
    {
        this.orientation = orientation;

        var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");
        animator.Scale = new Vector2((int)orientation * animator.Scale.x, animator.Scale.y);
    }

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

        if (timer < 0)
        {
            var proj_instance = (Node2D)proj.Instance();
            proj_instance.Rotation = Mathf.Deg2Rad(this.orientation == EntityOrientation.Left ? 180 : 0);
            AddChild(proj_instance);

            timer += 1 / rate;
        }
    }
}
