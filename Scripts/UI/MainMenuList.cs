using Godot;
// using System;

namespace Mimeva.UI;

public partial class MainMenuList : Label
{
    private string[] items = new string[] { $"Starting Difficulty {LevelGenVariables.LevelDifficulty}", $"Number of Levels: {LevelGenVariables.NumLevels}" };
    private int cursor = 0;

    public override void _Ready()
    {
        string label = "";
        // label = $"Starting Difficulty:     {LevelGenVariables.LevelDifficulty}\nNumber of Levels:     {LevelGenVariables.NumLevels}\n";
        for (int i = 0; i < items.Length; i++)
        {
            if (cursor == i)
            {
                label += "=>";
            }
            label += items[i];
        }

    }

    public override void _Process(double delta)
    {


    }
}
