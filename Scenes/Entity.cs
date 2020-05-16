using Godot;
using System;

public interface Entity {

    Movable GetMovable();

    Slowable GetSlowable();
    
}