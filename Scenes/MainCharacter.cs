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

    private bool jump;

    [Export]
    public int speed = 200;
    [Export]
    public int gravity = 40;
    [Export]
    public int jumpStrength = 600;


    private float dashTimer;
    private float dashTime = 0.15f;
    private float dashSpeed = 500;

    private float dashForce = 150;
    

    private float maximumVerticalVelocity = 400;

    private float verticalAcceleration = 0;


    private float horizontalVelocity = 0;
    private float horizontalAcceleration = 0;
    private float horizontalDrag = 30;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    private PackedScene proj = (PackedScene) GD.Load("res://scenes/Proj.tscn");

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        
        if (IsDashing()){
            dashTimer -= delta;
            
            horizontalVelocity = ((currentOrientation == CharacterOrientation.Right) ? 1:-1) * dashSpeed;

            this.MoveAndSlide(new Vector2(((currentOrientation == CharacterOrientation.Right) ? 1:-1) * dashSpeed,0));
        
        } else {
            this.UpdateVelocity(delta);

            this.UpdatePowerState();

            this.MoveAndSlide(velocity);
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

    private bool IsDashing(){
        return dashTimer > 0;
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

        

        /*if (this.IsOnFloor())
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y = Mathf.Min(this.maximumVerticalVelocity, velocity.y + this.gravity * delta);
        }*/

        if (jump){
            this.UpdateState(CharacterState.Jumping);
            verticalAcceleration = -jumpStrength;
            jump = false;
            velocity.y = 0;
        } else {
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

        if (horizontalVelocity > 0){
            horizontalVelocity -= Mathf.Min(horizontalDrag,horizontalVelocity);
        }
        if (horizontalVelocity < 0){
            horizontalVelocity += Mathf.Max(horizontalDrag,horizontalVelocity);
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
        var proj_instance = (Node2D) proj.Instance();
        proj_instance.Rotation = Mathf.Deg2Rad((currentOrientation == CharacterOrientation.Right)? 0 : 180);
        GetParent().AddChild(proj_instance);
        proj_instance.Position = Position;
    }

    private void DoJump()
    {
        if (!this.jumping && this.IsOnSomething())
        {
            this.UpdateState(CharacterState.Jumping);
            //this.velocity.y = -this.jumpStrength;
            jump = true;

            this.GetNode<AnimatedSprite>("AnimatedSprite").Play("jump");
        }
        else if (!hasDoubleJump && !this.IsOnSomething())
        {
            this.hasDoubleJump = true;
            //this.velocity.y = -this.jumpStrength;

            jump = true;
        }
        
    }

    private void DoTime()
    {
        // TODO
    }

    private void DoDash()
    {
        horizontalAcceleration = ((currentOrientation == CharacterOrientation.Right) ? 1:-1) * dashForce;
        dashTimer = dashTime;
    }
}
