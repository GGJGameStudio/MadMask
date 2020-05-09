using Godot;
using System;
using System.Collections.Generic;

public class MainCharacter : KinematicBody2D
{
    private readonly Vector2 UpDirection = new Vector2(0, -1);

    private Vector2 velocity = new Vector2();
    private CharacterState currentState = CharacterState.Idle;
    private CharacterOrientation currentOrientation = CharacterOrientation.Right;

    private bool jump;

    [Export]
    public int speed = 200;
    [Export]
    public int gravity = 30;
    [Export]
    public int jumpStrength = 600;

    private float wallJumpForce = 400;

    private float wallJumpStunDuration = 0.10f;

    private float maximumVerticalVelocity = 400;
    private float horizontalVelocity = 0;
    private float horizontalDrag = 30;

    private float airHorizontalDrag = 15;

    private Vector2 acceleration;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    private Dash dash;

    private float stunTimer;


    public override void _Ready()
    {
        dash = GetNode("Dash") as Dash;
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        if (dash.IsDashing())
        {
            var dashVelocity = new Vector2();
            if (IsOnFloor()){
                dashVelocity.x = ((currentOrientation == CharacterOrientation.Right) ? 1 : -1) * dash.GetSpeed();
            } else {
                dashVelocity.x = ((currentOrientation == CharacterOrientation.Right) ? 1 : -1) * dash.GetSpeed() * 0.4f;
                dashVelocity.y = dash.GetSpeed();
            }

            horizontalVelocity = dashVelocity.x;
            velocity.y = dashVelocity.y;

            this.MoveAndSlide(dashVelocity, UpDirection);
        }
        else
        {
            this.UpdateVelocity(delta);

            this.UpdatePowerState();

            this.MoveAndSlide(velocity, UpDirection);
        }

        if (this.currentState == CharacterState.Jumping && this.IsOnSomething())
        {
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
        #region horizontal velocity

        var horizontalSpeed = 0f;

        if (IsStunned()){
            stunTimer -= delta;
        } else {
            horizontalSpeed -= Input.GetActionStrength("character_move_left") * speed;
            horizontalSpeed += Input.GetActionStrength("character_move_right") * speed;
        }

        if (horizontalSpeed == 0)
        {
            this.UpdateState(CharacterState.Idle);
        }
        else
        {
            this.UpdateState(CharacterState.Running);
            this.UpdateOrientation(horizontalSpeed > 0 ? CharacterOrientation.Right : CharacterOrientation.Left);
        }

        horizontalVelocity += acceleration.x;

        var drag = horizontalDrag;
        if (!IsOnFloor()){
            drag = airHorizontalDrag;
        }

        if (horizontalVelocity > 0)
        {
            horizontalVelocity -= Mathf.Min(drag, horizontalVelocity);
        }
        if (horizontalVelocity < 0)
        {
            horizontalVelocity += Mathf.Max(drag, horizontalVelocity);
        }

        velocity.x = horizontalSpeed + horizontalVelocity;

        #endregion

        #region vertical velocity

        if (jump) // TODO jump plus haut en restant
        {
            this.UpdateState(CharacterState.Jumping);
            acceleration.y = -jumpStrength;
            jump = false;
            velocity.y = 0;
        }
        else
        {
            acceleration.y = gravity;
        }

        velocity.y += acceleration.y;
        if (velocity.y > maximumVerticalVelocity) velocity.y = maximumVerticalVelocity;

        #endregion

        // reset acceleration
        acceleration = Vector2.Zero;
    }

    private void UpdateState(CharacterState state)
    {
        if (this.currentState != state)
        {
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            if (state == CharacterState.Jumping)
            {
                this.currentState = state;
                animator.Play("jump");
            }
            else if (this.IsOnSomething())
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
            this.currentOrientation = orientation;

            var characterAnimator = this.GetNode<AnimatedSprite>("AnimatedSprite");
            characterAnimator.Scale = new Vector2(-characterAnimator.Scale.x, characterAnimator.Scale.y);

            foreach (var mask in this.availableMasks)
            {
                var spriteMask = mask.GetNode<Sprite>("Sprite");
                spriteMask.Scale = characterAnimator.Scale;
            }
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
        if (this.IsOnSomething())
        {
            jump = true;

            if (!this.IsOnFloor() && this.IsOnWall()){
                Bump(new Vector2(((currentOrientation == CharacterOrientation.Right) ? -1 : 1) * 500f, 0), 0.25f);
            }
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

    public void Bump(Vector2 force, float stunDuration){
        acceleration += force;
        Stun(stunDuration);
    }

    public void Stun(float stunDuration){ // Can't control character
        stunTimer = stunDuration;
    }

    public bool IsStunned(){
        return stunTimer > 0;
    }

    public bool IsDashing()
    {
        return dash.IsDashing();
    }
}
