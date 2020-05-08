using Godot;
using System;

public class MainCharacter : KinematicBody2D
{
    private const float DeadZone = 0.2f;

    private Vector2 velocity = new Vector2();

    [Export] 
    public int speed = 200;

    public void UpdateVelocity()
    {
        velocity = new Vector2();

        var xAxis = Input.GetJoyAxis(0, (int)JoystickList.Axis0);

        if (Mathf.Abs(xAxis) > DeadZone)
        {
            velocity.x += xAxis;
        }

        velocity = velocity * speed;
    }

    public override void _PhysicsProcess(float delta)
    {
        this.UpdateVelocity();

        velocity = this.MoveAndSlide(velocity);
    }
}
