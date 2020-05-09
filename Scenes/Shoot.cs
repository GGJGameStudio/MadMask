using Godot;
using System;

public class Shoot : Node2D
{
    private PackedScene proj = (PackedScene)GD.Load("res://scenes/Proj.tscn");

    public float rate = 1.5f;

    private float cooldown;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (cooldown > 0){
            cooldown -= delta;
        }
    }

    private bool ShootReady(){
        return cooldown <= 0;
    }

    public void Activate(CharacterOrientation orientation){
        if (ShootReady()){
            var proj_instance = (Node2D)proj.Instance();
            proj_instance.Rotation = Mathf.Deg2Rad((orientation == CharacterOrientation.Right) ? 0 : 180);
            GetParent().GetParent().AddChild(proj_instance);
            proj_instance.GlobalPosition = GlobalPosition;

            cooldown = 1 / rate;
        }
    }
}
