using Godot;
using System;

public class Slow : Area2D
{
    private bool active;

    private float slowTimer;
    private float cooldownTimer;

    private float slowDuration = 4f;

    private Sprite fx;
    private CollisionShape2D shape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        active = false;

        this.Connect("area_entered", this, nameof(_onSlowAreaEnter));
        this.Connect("area_exited", this, nameof(_onSlowAreaExit));

        this.Connect("body_entered", this, nameof(_onSlowBodyEnter));
        this.Connect("body_exited", this, nameof(_onSlowBodyExit));

        fx = GetNode<Sprite>("Sprite");
        fx.Visible = false;

        shape = GetNode<CollisionShape2D>("Shape");
        shape.Disabled = true;
    }

    public override void _Process(float delta)
    {
        if (active)
        {
            slowTimer -= delta;

            if (slowTimer <= 0)
            {
                Stop();
            }
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= delta;
        }

    }


    public void _onSlowAreaEnter(Area2D area)
    {
        if (active && area is Entity obj)
        {
            if (obj.GetSlowable() != null){
                obj.GetSlowable().Slow = true;
            }
        }
    }

    public void _onSlowAreaExit(Area2D area)
    {
        if (area is Entity obj)
        {
            if (obj.GetSlowable() != null){
                obj.GetSlowable().Slow = false;
            }
        }
    }

    public void _onSlowBodyEnter(Node node)
    {
        if (active && node is Entity obj)
        {
            if (obj.GetSlowable() != null){
                obj.GetSlowable().Slow = true;
            }
        }
    }

    public void _onSlowBodyExit(Node node)
    {
        if (node is Entity obj)
        {
            if (obj.GetSlowable() != null){
                obj.GetSlowable().Slow = false;
            }
        }
    }

    public bool IsSlowReady()
    {
        return !active && cooldownTimer <= 0;
    }

    public void Activate()
    {
        if (!active && IsSlowReady())
        {
            active = true;
            slowTimer = slowDuration;
            fx.Visible = true;
            shape.Disabled = false;
        } else {
            if (active){
                Stop();
            }     
        }
    }

    private void Stop()
    {
        active = false;
        fx.Visible = false;
        shape.Disabled = true;

        cooldownTimer = slowDuration - slowTimer;
        slowTimer = 0;
    }
}
