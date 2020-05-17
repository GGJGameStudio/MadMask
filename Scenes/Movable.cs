using Godot;
using System;

public class Movable : Node2D {
    
    [Export]
    public float moveDistanceX;
    [Export]
    public float moveDistanceY;
    [Export]
    public float moveSpeed;
    [Export]
    public float delay;

    public float startTimer;

    private Vector2 startPosition;
    private float timer;

    private bool comebackPhase;

    public Slowable Slowable {get; set;}

    private Node2D entity;


    public Movable(){
        Slowable = new Slowable();
    }
    
    public override void _Ready()
    {
        entity = GetParent() as Node2D;
        comebackPhase = false;
        startPosition = entity.GlobalPosition;

        startTimer = delay;
    }

    public override void _Process(float delta)
    {
        if (!Freeze.Frozen){
            if (startTimer > 0){
                startTimer -= delta;
            } else {
                if (moveSpeed > 0){
                    var moveTarget = new Vector2(moveDistanceX, moveDistanceY) + startPosition;
                    var moveDistance = moveTarget.DistanceTo(startPosition);
                    var movePeriod = moveDistance / moveSpeed;
                    
                    if (!comebackPhase){
                        timer += (Slowable.Slow ? delta * Slowable.SpeedSlow : delta);
                        if (timer >= movePeriod){
                            timer = movePeriod;
                            comebackPhase = true;
                        }
                    } else {
                        timer -= (Slowable.Slow ? delta * Slowable.SpeedSlow : delta);
                        if (timer <= 0){
                            timer = 0;
                            comebackPhase = false;
                        }   
                    }

                    entity.GlobalPosition = startPosition.LinearInterpolate(moveTarget, timer / movePeriod);
                }
            }
        }
        
    }
}