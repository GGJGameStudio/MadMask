using Godot;
using System;

public class Slow : Area2D
{
    private bool active;

    private float slowTimer;
    private float cooldownTimer;

    private float slowDuration = 3f;
    private float slowCooldown = 3f;

    private AnimatedSprite fx;
    private CollisionShape2D shape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        active = false;

        this.Connect("area_entered", this, nameof(_onSlowEnter));
        this.Connect("area_exited", this, nameof(_onSlowExit));

        fx = GetNode("SlowEffect") as AnimatedSprite;
        fx.Visible = false;

        shape = GetNode("Shape") as CollisionShape2D;
        shape.Disabled = true;
    }

    public override void _Process(float delta)
    {
        if (active){
            slowTimer -= delta;

            if (slowTimer <= 0){
                Stop();
                cooldownTimer = slowTimer + slowCooldown;
            }
        }

        if (cooldownTimer > 0){
            cooldownTimer -= delta;
        }

    }

    
    public void _onSlowEnter(Area2D area)
    {
        if (active && area is Proj proj){
            proj.SetSlow(true);
        }
    }

    public void _onSlowExit(Area2D area)
    {
        if (area is Proj proj){
            proj.SetSlow(false);
        }
    }

    public bool IsSlowReady(){
        return !active && cooldownTimer <= 0;
    }

    public void Activate(){
        if (IsSlowReady()){
            active = true;
            slowTimer = slowDuration;
            fx.Visible = true;
            shape.Disabled = false; 
        }
    }

    private void Stop(){
        active = false;
        fx.Visible = false;
        shape.Disabled = true;
    }
}
