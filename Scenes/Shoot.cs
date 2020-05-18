using Godot;
using System;

public class Shoot : Node2D
{
    private PackedScene proj = (PackedScene)GD.Load("res://Scenes/Proj.tscn");

    public float rate = 1.5f;

    private float cooldown;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (cooldown > 0)
        {
            cooldown -= delta;
        }
    }

    private bool ShootReady()
    {
        return cooldown <= 0;
    }

    public bool Activate(EntityOrientation orientation, bool boosted)
    {
        if (ShootReady())
        {
            var proj_instance = (Proj)proj.Instance();
            proj_instance.Rotation = Mathf.Deg2Rad((orientation == EntityOrientation.Right) ? 0 : 180);
            if (boosted)
            {
                proj_instance.SetBoosted();
            }
            GetParent().GetParent().AddChild(proj_instance);
            proj_instance.GlobalPosition = GlobalPosition;


            cooldown = 1 / rate;
            return true;
        }

        return false;
    }
}
