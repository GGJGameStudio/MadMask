using Godot;
using System;

public class MainCharacter : KinematicBody2D
{
    private const float DeadZone = 0.2f;

    private Vector2 velocity = new Vector2();
    private CharacterState currentState = CharacterState.Idle;
    private CharacterOrientation currentOrientation = CharacterOrientation.Right;

    [Export]
    public int speed = 200;

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateVelocity();

        velocity = this.MoveAndSlide(velocity);
    }

    private void UpdateVelocity()
    {
        velocity = new Vector2(0, 1);

        Console.WriteLine("left: " + Input.GetActionStrength("character_move_left"));
        Console.WriteLine("right: " + Input.GetActionStrength("character_move_right"));

        velocity.x -= Input.GetActionStrength("character_move_left");
        velocity.x += Input.GetActionStrength("character_move_right");

        velocity = velocity.Normalized() * speed;

        if (velocity.x == 0)
        {
            this.UpdateState(CharacterState.Idle);
        }
        else
        {
            this.UpdateState(CharacterState.Running);
            this.UpdateOrientation(velocity.x > 0 ? CharacterOrientation.Right : CharacterOrientation.Left);
        }
    }

    private void UpdateState(CharacterState state)
    {
        if (this.currentState != state)
        {
            this.currentState = state;
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            switch (state)
            {
                case CharacterState.Idle:
                    animator.Play("idle");
                    break;
                case CharacterState.Running:
                    animator.Play("run");
                    break;
                case CharacterState.Jumping:
                    animator.Play("jump");
                    break;
            }
        }
    }

    private void UpdateOrientation(CharacterOrientation orientation)
    {
        if (this.currentOrientation != orientation)
        {
            this.currentOrientation = orientation;
            this.Scale = orientation == CharacterOrientation.Right ? new Vector2(1, 1) : new Vector2(-1, 1);
        }
    }
}
