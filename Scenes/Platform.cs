using Godot;
using System;

public class Platform : KinematicBody2D
{
    protected bool slow = false;
    
    protected float speedSlow = 0.5f;

    [Export]
    public float moveDistanceX;
    [Export]
    public float moveDistanceY;
    [Export]
    public float moveSpeed;

    private Vector2 startPosition;
    private float timer;

    private bool comebackPhase;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        comebackPhase = false;
        startPosition = Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (moveSpeed > 0){


            var moveTarget = new Vector2(moveDistanceX, moveDistanceY) + startPosition;
            var moveDistance = moveTarget.DistanceTo(startPosition);
            var movePeriod = moveDistance / moveSpeed;
            
            if (!comebackPhase){
                timer += (slow ? delta * speedSlow : delta);
                if (timer >= movePeriod){
                    timer = movePeriod;
                    comebackPhase = true;
                }
            } else {
                timer -= (slow ? delta * speedSlow : delta);
                if (timer <= 0){
                    timer = 0;
                    comebackPhase = false;
                }   
            }

            Position = startPosition.LinearInterpolate(moveTarget, timer / movePeriod);
        }
    }

    
    public void SetSlow(bool slow)
    {
        this.slow = slow;
    }
}
