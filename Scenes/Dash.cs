using Godot;
using System;

public class Dash : Node2D
{
    private float timer;
    private float duration = 0.15f;
    private float speed = 800f;
    private float cooldown = 0.6f;

    private float cooldownTimer;

    private float dashBoostTime = 0.5f;
    private float dashBoostTimer;

    public bool verticalDash;

    public EntityOrientation dashDir;

    MainCharacter character;
    AnimatedSprite characterSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        character = GetParent() as MainCharacter;
        characterSprite = character.GetNode("AnimatedSprite") as AnimatedSprite;
    }

    public override void _Process(float delta)
    {
        if (IsDashing()){
            timer -= delta;
            characterSprite.Modulate = new Color(1,1,1,0.2f);
        } else {
            characterSprite.Modulate = new Color(1,1,1,1);
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= delta;
        }

        if (IsBoosting()){
            dashBoostTimer -= delta;
        }
    }

    public bool IsDashing()
    {
        return timer > 0;
    }

    private bool IsDashReady()
    {
        return cooldownTimer <= 0;
    }

    public bool IsBoosting(){
        return dashBoostTimer > 0;
    }

    public void Activate(EntityOrientation orientation){
        if (IsDashReady()){
            timer = duration;
            cooldownTimer = cooldown;
            character.Stun(duration);
            dashBoostTimer = dashBoostTime;
            if (character.IsOnFloor()){
                verticalDash = false;
            } else {
                verticalDash = true;
            }
            dashDir = character.GetOrientation();
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
}
