using Godot;
using System;

namespace Mimeva;
public partial class StartGame : Label
{
    [Export] private string starting_scene_path = "";
    private float curr_alpha = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        curr_alpha += 2 * (float)delta;
        this.Modulate = new Godot.Color(this.Modulate.R, this.Modulate.G, this.Modulate.B, (1 + (float)Math.Sin(curr_alpha)) / 2);

        if (Input.IsActionJustPressed("up"))
        { // increase difficulty
            LevelGenVariables.LevelDifficulty = LevelGenVariables.LevelDifficulty < 10 ? LevelGenVariables.LevelDifficulty + 1 : 10;
        }
        else if (Input.IsActionJustPressed("down"))
        { // decrease difficulty
            LevelGenVariables.LevelDifficulty = LevelGenVariables.LevelDifficulty > 0 ? LevelGenVariables.LevelDifficulty - 1 : 0;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            try
            {
                GetTree().ChangeSceneToFile(starting_scene_path);
            }
            catch
            {
                GD.PrintErr("error loading scene!");
                GetTree().Quit();
            }
        }
    }
}
