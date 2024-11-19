using Godot;
using Mimeva;
using System;

namespace Mimeva.UI;

public partial class DifficultyLabel : RichTextLabel
{

    Player player;
    /*[Export] FontFile font_f;*/

    public override void _Ready()
    {
        this.Text = "Difficulty:   ";

        // Load font
        /*Font f = new Godot.Font();*/
        /*f.Theme.FontFile = font_f;*/


        // Adjust Appearance
        this.Position = new(10, 40);
        this.Size = new(100, 100);
        this.ScrollActive = false;
        /*this.PushFontSize(20);*/
        /*this.Theme.Set("normal_font", font_f);*/

    }

    public override void _Process(double delta)
    {
        this.Text = $"Difficulty: {LevelGenVariables.LevelDifficulty}";
    }
}
