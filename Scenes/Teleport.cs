using Godot;
using System;

public class Teleport : Area2D
{
    private bool teleportInProgress;

    [Signal]
    public delegate void OnMainCharacterTeleportation();

    public void _on_Area2D_body_entered(Node body)
    {
        if (body is MainCharacter mainCharacter)
        {
            var animator = this.GetNode<AnimatedSprite>("AnimatedSprite");

            mainCharacter.Stun(1);

            animator.Play("activated");

            this.teleportInProgress = true;
        }
    }

    public void _on_AnimatedSprite_animation_finished()
    {
        if (this.teleportInProgress)
        {
            this.teleportInProgress = false;
            this.EmitSignal(nameof(OnMainCharacterTeleportation));
            this.GetNode<AnimatedSprite>("AnimatedSprite").Play("standBy");
        }
    }
}
