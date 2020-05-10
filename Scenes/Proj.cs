using Godot;
using System;

public class Proj : Slowable
{
    private float baseSpeed = 200f;
    private float boostSpeed = 300f;

    private float lifeTime = 2.5f;

    private float speed;


    private bool boosted;

    private float timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (boosted){
            speed = boostSpeed;
        } else {
            speed = baseSpeed;
        }

        timer = lifeTime;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * (slow ? speed * speedSlow : speed) * delta;

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
}
