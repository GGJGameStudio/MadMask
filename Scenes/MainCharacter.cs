using Godot;
using System;
using System.Collections.Generic;

public class MainCharacter : KinematicBody2D
{
    private Vector2 velocity = new Vector2();
    private CharacterState currentState = CharacterState.Idle;
    private CharacterOrientation currentOrientation = CharacterOrientation.Right;

    [Export]
    public int speed = 200;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    public override void _Ready()
    {
        foreach(Node spike in GetTree().GetNodesInGroup("Spikes")){
            spike.Connect("body_entered", this, nameof(_onSpikeCollide));
        }
    }
    

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        this.UpdateVelocity();

        velocity = this.MoveAndSlide(velocity);
    }

    private void UpdateMask()
    {
        foreach (var mask in this.availableMasks)
        {
            if (this.CurrentMask != mask && Input.IsActionJustPressed(mask.AssociatedAction))
            {
                this.CurrentMask?.ChangeState(false);
                this.CurrentMask = mask;
                this.CurrentMask.ChangeState(true);

                return;
            }
        }
    }

    private void UpdateVelocity()
    {
        velocity = new Vector2(0, 1);

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
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            this.currentOrientation = orientation;
            animator.Scale = orientation == CharacterOrientation.Right ? new Vector2(1, 1) : new Vector2(-1, 1);
        }
    }

    public void _onSpikeCollide(Node body){
        GD.Print("dead");
    }
}
