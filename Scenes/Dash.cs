using Godot;
using System;

public class Dash : Node2D
{
    private float timer;
    private float duration = 0.15f;
    private float speed = 500;
    private float force = 150;

    private float cooldown = 0.6f;

    private float cooldownTimer;

    MainCharacter character;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        character = GetParent() as MainCharacter;
    }

    public override void _Process(float delta)
    {
        if (IsDashing()){
            timer -= delta; 
        }

        if (cooldownTimer > 0){
            cooldownTimer -= delta;
        }
    }

    public bool IsDashing()
    {
        return timer > 0;
    }

    private bool IsDashReady(){
        return cooldownTimer <= 0;
    }

    public void Activate(CharacterOrientation orientation){
        if (IsDashReady()){
            character.SetHorizontalAcceleration(((orientation == CharacterOrientation.Right) ? 1 : -1) * force);
            timer = duration;
            cooldownTimer = cooldown;
        }
    }

    public float GetSpeed(){
        return speed;
    }
}
