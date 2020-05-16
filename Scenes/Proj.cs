using Godot;
using System;

public class Proj : Area2D, Entity
{
    private float baseSpeed = 150f;
    private float boostSpeed = 250f;

    private float lifeTime = 2.5f;

    private float speed;


    private bool boosted;

    private float timer;

    private Slowable Slowable;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (boosted){
            speed = boostSpeed;
        } else {
            speed = baseSpeed;
        }

        timer = lifeTime;

        Slowable = new Slowable();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * (Slowable.Slow ? speed * Slowable.SpeedSlow : speed) * delta;

        timer -= delta;

        if (timer <= 0){
            QueueFree();
        }
    }

    public bool IsDirectionRight(){
        return Rotation == 0;
    }

    public void SetBoosted(){
        boosted = true;
    }

    public void _on_body_entered(Node body){
        if (body.GetType() == typeof(TileMap)){
            QueueFree();
        }
    }

    public Movable GetMovable(){
        return null;
    }

    public Slowable GetSlowable(){
        return Slowable;
    }
}
