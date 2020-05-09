using Godot;
using System;

public class Mask : Area2D
{
    [Export]
    public MaskType Type;

    public string AssociatedAction
    {
        get
        {
            switch (this.Type)
            {
                case MaskType.Dash:
                    return "wear_dash_mask";
                case MaskType.Time:
                    return "wear_time_mask";
                case MaskType.Jump:
                    return "wear_jump_mask";
                case MaskType.Shot:
                    return "wear_shot_mask";
                default:
                    return null;
            }
        }
    }

    public bool IsAffected { get; private set; }

    public void ChangeState(bool isWorn)
    {
        this.Visible = isWorn;
    }

    public void _on_Area2D_body_entered(Node body)
    {
        if (!this.IsAffected && body is MainCharacter mainCharacter)
        {
            CallDeferred(nameof(this.AffectMask), mainCharacter);
        }
    }

    private void AffectMask(MainCharacter mainCharacter)
    {
        this.Visible = false;
        this.IsAffected = true;

        this.GetParent().RemoveChild(this);
        mainCharacter.AddChild(this);
        mainCharacter.availableMasks.Add(this);

        this.Position = new Vector2();

        this.GetNode<Sprite>("Sprite").Scale = mainCharacter.GetNode<AnimatedSprite>("AnimatedSprite").Scale;
    }
}
