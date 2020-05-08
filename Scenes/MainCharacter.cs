using Godot;
using System;
using System.Collections.Generic;

public class MainCharacter : KinematicBody2D
{
    private Vector2 velocity = new Vector2();
    private CharacterState currentState = CharacterState.Idle;
    private CharacterOrientation currentOrientation = CharacterOrientation.Right;

    private bool jumping;
    private bool hasDoubleJump;

    [Export]
    public int speed = 200;
    [Export]
    public int gravity = 100;
    [Export]
    public int jumpStrength = 200;

    private float maximumVerticalVelocity = 400;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    public override void _Ready()
    {
        foreach (Node spike in GetTree().GetNodesInGroup("Spikes"))
        {
            spike.Connect("body_entered", this, nameof(_onSpikeCollide));
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        this.UpdateVelocity(delta);

        this.UpdatePowerState();

        this.MoveAndSlide(velocity);

        if (this.jumping && this.IsOnSomething())
        {
            this.hasDoubleJump = false;
            this.jumping = false;

            this.UpdateState(CharacterState.Idle);
        }
    }

    private bool IsOnSomething()
    {
        return this.IsOnFloor() || this.IsOnWall();
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

    private void UpdateVelocity(float delta)
    {
        #region vertical velocity

        if (this.IsOnFloor())
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y = Mathf.Min(this.maximumVerticalVelocity, velocity.y + this.gravity * delta);
        }

        #endregion

        #region horizontal velocity

        var horizontalVelocity = 0f;

        horizontalVelocity -= Input.GetActionStrength("character_move_left");
        horizontalVelocity += Input.GetActionStrength("character_move_right");

        if (horizontalVelocity == 0)
        {
            this.UpdateState(CharacterState.Idle);
        }
        else
        {
            this.UpdateState(CharacterState.Running);
            this.UpdateOrientation(horizontalVelocity > 0 ? CharacterOrientation.Right : CharacterOrientation.Left);
        }

        velocity.x = horizontalVelocity * speed;

        #endregion
    }

    private void UpdateState(CharacterState state)
    {
        if (this.currentState != state)
        {
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            if (state == CharacterState.Jumping)
            {
                this.currentState = state;
                this.jumping = true;
                animator.Play("jump");
            }
            else if (!this.jumping)
            {
                this.currentState = state;

                switch (state)
                {
                    case CharacterState.Idle:
                        animator.Play("idle");
                        break;
                    case CharacterState.Running:
                        animator.Play("run");
                        break;
                    case CharacterState.Jumping:

                        break;
                }
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

    private void UpdatePowerState()
    {
        if (Input.IsActionJustPressed("activate_power"))
        {
            switch (this.CurrentMask?.Type)
            {
                case MaskType.Dash:
                    this.DoDash();
                    break;
                case MaskType.Time:
                    this.DoTime();
                    break;
                case MaskType.Jump:
                    this.DoJump();
                    break;
                case MaskType.Shot:
                    this.DoShot();
                    break;
                default:
                    break;
            }
        }
    }

    private void DoShot()
    {
        // TODO
    }

    private void DoJump()
    {
        if (!this.jumping && this.IsOnSomething())
        {
            this.UpdateState(CharacterState.Jumping);
            this.velocity.y = -this.jumpStrength;

            this.GetNode<AnimatedSprite>("AnimatedSprite").Play("jump");
        }
        else if (!hasDoubleJump && !this.IsOnSomething())
        {
            this.hasDoubleJump = true;
            this.velocity.y = -this.jumpStrength;
        }
    }

    private void DoTime()
    {
        // TODO
    }

    private void DoDash()
    {
        // TODO
    }

    public void _onSpikeCollide(Node body)
    {
        GD.Print("dead");
    }
}
