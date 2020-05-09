using Godot;
using System;

public class JumpProjArea : Area2D
{
    MainCharacter character;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        character = GetParent() as MainCharacter;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void _on_area_entered(Area2D area)
    {
        if (area is Proj proj){
            GD.Print("test");
            character.AddJumpProj(proj);
        }
    }

    public void _on_area_exited(Area2D area)
    {
        if (area is Proj proj){
            GD.Print("test2");
            character.RemoveJumpProj(proj);
        }
    }
}
