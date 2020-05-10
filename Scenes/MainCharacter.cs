using Godot;
using System;
using System.Collections.Generic;

public class MainCharacter : KinematicBody2D
{
    private readonly Vector2 UpDirection = new Vector2(0, -1);

    private Vector2 velocity = new Vector2();

    private Vector2 finalVelocity;
    private EntityState currentState = EntityState.Idle;
    private EntityOrientation currentOrientation = EntityOrientation.Right;

    [Export]
    public int speed = 200;
    [Export]
    public int gravity = 30;

    public int jumpStrength = 300;
    public int jumpSecondaryStrength = 60;

    private bool jump;
    private bool wallJump;
    private float jumpTimer;

    private float jumpDuration = 0.15f;
    private float dashJumpBoost = 250;

    private float wallJumpForce = 500;
    private float wallJumpVForce = 650;

    private float wallJumpStunDuration = 0.25f;

    private float wallJumpSnapDuration = 0.2f;
    private float wallJumpSnapTimer;

    private EntityOrientation snapDir;

    private float maximumVerticalVelocity = 350;
    private float horizontalVelocity = 0;
    private float horizontalDrag = 50;

    private float airHorizontalDrag = 15;

    private float verticalWallDrag = 30f;


    private Vector2 acceleration;

    public List<Mask> availableMasks = new List<Mask>();

    public Mask CurrentMask;

    private Dash dash;
    private Shoot shoot;

    private float stunTimer;
    private float noGravityTimer;

    private List<Proj> jumpProj;


    public override void _Ready()
    {
        dash = GetNode("Dash") as Dash;
        shoot = GetNode("Shoot") as Shoot;

        jumpProj = new List<Proj>();
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateMask();

        if (dash.IsDashing())
        {
            var dashVelocity = new Vector2();
            if (IsOnFloor())
            {
                dashVelocity.x = (int)currentOrientation * dash.GetSpeed();
            }
            else
            {
                dashVelocity.x = (int)currentOrientation * dash.GetSpeed() * 0.4f;
                dashVelocity.y = dash.GetSpeed();
            }

            horizontalVelocity = dashVelocity.x;
            velocity.y = dashVelocity.y;

            finalVelocity = this.MoveAndSlide(dashVelocity, UpDirection);
        }
        else
        {
            this.UpdatePowerState();

            this.UpdateVelocity(delta);

            finalVelocity = this.MoveAndSlide(velocity, UpDirection);
        }

        if (this.currentState == EntityState.Jumping && this.IsOnSomething())
        {
            this.UpdateState(EntityState.Idle);
        }
    }

    private bool IsOnSomething()
    {
        return this.IsOnFloor() || this.IsOnWall() || IsOnWallSnap() || jumpProj.Count > 0;
    }

    private void UpdateMask()
    {
        foreach (var mask in this.availableMasks)
        {
            if (this.CurrentMask != mask && Input.IsActionJustPressed(mask.AssociatedAction))
            {
                this.ChangeMask(mask);

                return;
            }
        }

        if (this.availableMasks.Count > 0)
        {
            if (Input.IsActionJustPressed("wear_next_mask"))
            {
                var index = (this.availableMasks.IndexOf(this.CurrentMask) + 1) % this.availableMasks.Count;

                this.ChangeMask(this.availableMasks[index]);
            }
            else if (Input.IsActionJustPressed("wear_previous_mask"))
            {
                var index = (this.availableMasks.IndexOf(this.CurrentMask) - 1) % this.availableMasks.Count;

                if (index < 0)
                {
                    index += this.availableMasks.Count;
                }

                this.ChangeMask(this.availableMasks[index]);
            }
        }
    }

    private void ChangeMask(Mask newMask)
    {
        this.CurrentMask?.ChangeState(false);
        this.CurrentMask = newMask;
        this.CurrentMask.ChangeState(true);
    }

    private void UpdateVelocity(float delta)
    {
        #region horizontal velocity

        if (IsOnWall() && finalVelocity.x == 0){
            snapDir = currentOrientation;
        }

        var horizontalSpeed = 0f;

        if (IsStunned())
        {
            stunTimer -= delta;
        }
        else
        {
            horizontalSpeed -= Input.GetActionStrength("character_move_left") * speed;
            horizontalSpeed += Input.GetActionStrength("character_move_right") * speed;
        }

        if (horizontalSpeed == 0)
        {
            this.UpdateState(EntityState.Idle);
        }
        else
        {
            this.UpdateState(EntityState.Running);
            this.UpdateOrientation(horizontalSpeed > 0 ? EntityOrientation.Right : EntityOrientation.Left);
        }

        if (IsJumping() && dash.IsBoosting())
        {
            acceleration.x += dashJumpBoost * ((currentOrientation == EntityOrientation.Right) ? 1 : -1);
        }

        horizontalVelocity += acceleration.x;

        var hdrag = horizontalDrag;
        if (!IsOnFloor())
        {
            hdrag = airHorizontalDrag;
        }

        if (horizontalVelocity > 0)
        {
            horizontalVelocity -= Mathf.Min(hdrag, horizontalVelocity);
        }
        if (horizontalVelocity < 0)
        {
            horizontalVelocity += Mathf.Max(hdrag, horizontalVelocity);
        }

        velocity.x = horizontalSpeed + horizontalVelocity;

        #endregion

        #region vertical velocity

        var vdrag = 0f;
        if (IsOnWall()){
            wallJumpSnapTimer = wallJumpSnapDuration;
            vdrag = verticalWallDrag;
        } else {
            wallJumpSnapTimer -= delta;
        }
        
        if (wallJump)
        {
            this.UpdateState(EntityState.Jumping);
            velocity.y = 0;
            acceleration.y += -wallJumpVForce;
            wallJump = false;

        }
        
        if (jump){
            this.UpdateState(EntityState.Jumping);
            velocity.y = 0;
            acceleration.y += -jumpStrength;
            jump = false;
        }

        if (!jump && !wallJump){
            if (noGravityTimer > 0){
                noGravityTimer -= delta;
            } else {
                acceleration.y += gravity;
            }   
        }

        if (IsJumping()){
            if (Input.IsActionPressed("activate_power") && CurrentMask.Type == MaskType.Jump){
                acceleration.y += -jumpSecondaryStrength;
            }
            jumpTimer -= delta;
        }

        velocity.y += acceleration.y;

        if (velocity.y > 0) velocity.y -= Mathf.Min(vdrag, velocity.y);
        if (velocity.y > maximumVerticalVelocity) velocity.y = maximumVerticalVelocity;

        #endregion

        // reset acceleration
        acceleration = Vector2.Zero;
    }

    private void UpdateState(EntityState state)
    {
        if (this.currentState != state)
        {
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            if (state == EntityState.Jumping)
            {
                this.currentState = state;
                animator.Play("jump");
            }
            else if (this.IsOnSomething())
            {
                this.currentState = state;

                switch (state)
                {
                    case EntityState.Idle:
                        animator.Play("idle");
                        break;
                    case EntityState.Running:
                        animator.Play("run");
                        break;
                }
            }
        }
    }

    private void UpdateOrientation(EntityOrientation orientation)
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
        if (Input.IsActionJustPressed("activate_power") && !IsStunned())
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
        var result = shoot.Activate(currentOrientation, dash.IsBoosting());
        if (result){
            velocity = Vector2.Zero;
            StopGravity(0.25f);
            Stun(0.25f);
        }
    }

    private void DoJump()
    {
        if (this.IsOnSomething())
        {
            if (this.IsOnFloor()){
                jump = true;
                jumpTimer = jumpDuration;
            }

            if (!this.IsOnFloor() && this.IsOnWall())
            {
                // wall jump
                Bump(new Vector2(-(int)snapDir * wallJumpForce, 0), wallJumpStunDuration);
                wallJump = true;
                velocity.y = 0;
            }

            if (!this.IsOnFloor() && !this.IsOnWall() && this.IsOnWallSnap())
            {
                // wall jump
                Bump(new Vector2(-(int)snapDir * wallJumpForce, 0), wallJumpStunDuration);
                wallJump = true;
                velocity.y = 0;
            }

            if (!this.IsOnFloor() && !this.IsOnWall() && !this.IsOnWallSnap() && jumpProj.Count > 0){
                // proj jump
                jumpProj[0].QueueFree();
                jumpProj.RemoveAt(0);
                jump = true;
                jumpTimer = jumpDuration;
            }

            if (!jump){
                /*GD.Print("fail");
                GD.Print("floor " + this.IsOnFloor());
                GD.Print("wall " + this.IsOnWall());
                GD.Print("snap " + this.IsOnWallSnap());
                GD.Print("snapdir " + snapDir.ToString());
                GD.Print("dir " + currentOrientation);*/
            }
        }
    }

    private void DoTime()
    {
        (GetNode("SlowArea") as Slow).Activate();
    }

    private void DoDash()
    {
        dash.Activate(currentOrientation);
    }

    public void Bump(Vector2 force, float stunDuration)
    {
        velocity = Vector2.Zero;
        acceleration += force;
        Stun(stunDuration);
    }

    public void Stun(float stunDuration)
    { // Can't control character
        stunTimer = stunDuration;
    }

    public void StopGravity(float duration){
        noGravityTimer = duration;
    }

    public bool IsStunned()
    {
        return stunTimer > 0;
    }

    public bool IsDashing()
    {
        return dash.IsDashing();
    }

    public bool IsOnWallSnap(){
        return wallJumpSnapTimer > 0;
    }

    public bool IsJumping(){
        return jumpTimer > 0;
    }
    public void AddJumpProj(Proj proj){
        jumpProj.Add(proj);
    }

    public void RemoveJumpProj(Proj proj){
        jumpProj.Remove(proj);
    }

    public void Restart(){
        GetTree().ChangeScene("res://" + Game.currentLevel + ".tscn");
    }
}
