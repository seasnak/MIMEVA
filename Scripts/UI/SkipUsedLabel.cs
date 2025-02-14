using Godot;

using Mimeva.PCG;

namespace Mimeva.UI;

public partial class SkipUsedLabel : TextureRect
{
    [Export] private Texture2D skip_used_sprite;
    [Export] private Texture2D skip_notused_sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (LevelGenVariables.PlayerHasSkipped)
        {
            this.Texture = skip_used_sprite;
        }
        else
        {
            this.Texture = skip_notused_sprite;
        }


    }
}
