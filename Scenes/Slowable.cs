using Godot;
using System;

public abstract class Slowable : Area2D {
    
    protected bool slow = false;
    
    protected float speedSlow = 0.5f;
    
    public void SetSlow(bool slow)
    {
        this.slow = slow;
    }
}