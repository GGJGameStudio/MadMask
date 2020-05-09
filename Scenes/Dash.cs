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
            timer = duration;
            cooldownTimer = cooldown;
        }
    }

    public float GetSpeed(){
        return speed;
    }
}
