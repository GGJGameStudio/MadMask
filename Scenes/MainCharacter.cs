using Godot;
using System;
using System.Collections.Generic;

public class MainCharacter : KinematicBody2D
{
    private readonly Vector2 UpDirection = new Vector2(0, -1);

    private Vector2 velocity = new Vector2();
    private CharacterState currentState = CharacterState.Idle;
    private CharacterOrientation currentOrientation = CharacterOrientation.Right;

    private bool jumping;
    private bool hasDoubleJump;

    private bool jump;

    [Export]
    public int speed = 200;
    [Export]
    public int gravity = 40;
    [Export]
    public int jumpStrength = 600;

    private float maximumVerticalVelocity = 400;
    private float verticalAcceleration = 0;

    private float horizontalVelocity = 0;
    private float horizontalAcceleration = 0;
    private float horizontalDrag = 30;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    private Dash dash;


    public override void _Ready()
    {
        dash = GetNode("Dash") as Dash;
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        if (dash.IsDashing())
        {
            horizontalVelocity = ((currentOrientation == CharacterOrientation.Right) ? 1 : -1) * dash.GetSpeed();

            this.MoveAndSlide(new Vector2(((currentOrientation == CharacterOrientation.Right) ? 1 : -1) * dash.GetSpeed(), 0), UpDirection);
        }
        else
        {
            this.UpdateVelocity(delta);

            this.UpdatePowerState();

            this.MoveAndSlide(velocity, UpDirection);
        }

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

        if (jump)
        {
            this.UpdateState(CharacterState.Jumping);
            verticalAcceleration = -jumpStrength;
            jump = false;
            velocity.y = 0;
        }
        else
        {
            verticalAcceleration = gravity;
        }

        velocity.y += verticalAcceleration;
        if (velocity.y > maximumVerticalVelocity) velocity.y = maximumVerticalVelocity;

        #endregion

        #region horizontal velocity

        var horizontalSpeed = 0f;

        horizontalSpeed -= Input.GetActionStrength("character_move_left") * speed;
        horizontalSpeed += Input.GetActionStrength("character_move_right") * speed;

        if (horizontalSpeed == 0)
        {
            this.UpdateState(CharacterState.Idle);
        }
        else
        {
            this.UpdateState(CharacterState.Running);
            this.UpdateOrientation(horizontalSpeed > 0 ? CharacterOrientation.Right : CharacterOrientation.Left);
        }

        if (horizontalVelocity > 0)
        {
            horizontalVelocity -= Mathf.Min(horizontalDrag, horizontalVelocity);
        }
        if (horizontalVelocity < 0)
        {
            horizontalVelocity += Mathf.Max(horizontalDrag, horizontalVelocity);
        }

        velocity.x = horizontalSpeed + horizontalVelocity;

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
        (GetNode("Shoot") as Shoot).Activate(currentOrientation);
    }

    private void DoJump()
    {
        if (!this.jumping && this.IsOnSomething())
        {
            this.UpdateState(CharacterState.Jumping);
            jump = true;
        }
        else if (!hasDoubleJump && !this.IsOnSomething())
        {
            this.hasDoubleJump = true;
            jump = true;
        }

    }

    private void DoTime()
    {
        (GetNode("SlowArea") as Slow).Activate();
    }

    private void DoDash()
    {
        (GetNode("Dash") as Dash).Activate(currentOrientation);
    }

    public void SetHorizontalAcceleration(float acceleration){
        horizontalAcceleration = acceleration;
    }

    public bool IsDashing()
    {
        return dash.IsDashing();
    }
}
