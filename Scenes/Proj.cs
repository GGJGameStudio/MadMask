using Godot;
using System;

public class Proj : Area2D
{
    private float speed = 200;

    private bool slow = false;

    private float speedSlow = 0.5f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (Slow slow in GetTree().GetNodesInGroup("Slow"))
        {
            slow.Connect("area_entered", this, nameof(_onSlowEnter));
            slow.Connect("area_exited", this, nameof(_onSlowExit));
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * (slow ? speed * speedSlow : speed) * delta;
    }

    public void _onSlowEnter(Area2D area)
    {
        //GD.Print("enter : " + area.Name);
        slow = true;
    }

    public void _onSlowExit(Area2D area)
    {
        slow = false;
    }

    public void SetSlow(bool slow){
        this.slow = slow;
    }
}
