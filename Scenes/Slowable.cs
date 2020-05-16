using Godot;
using System;

public class Slowable {
    
    public bool Slow {get; set;}
    
    public float SpeedSlow {get; set;} 
    
    public Slowable(){
        SpeedSlow = 0.5f;
    }
}