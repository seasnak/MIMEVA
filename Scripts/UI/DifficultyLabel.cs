using Godot;
using Mimeva;
using System;

namespace Mimeva.UI;

public partial class DifficultyLabel : Label //RichTextLabel
{

    Player player;

    public override void _Ready()
    {
        this.Text = "Difficulty:   ";

        /*this.AddThemeFontOverride("", new Godot.Font("res://Fonts/monogram/monogram.ttf"));*/
        /*this.AddThemeFontSizeOverride("", 20);*/

        // Adjust Appearance
        this.Position = new(15, 40);
        this.Size = new(100, 100);

    }

    public override void _Process(double delta)
    {
        this.Text = $"Difficulty: {LevelGenVariables.LevelDifficulty}";
    }
}
