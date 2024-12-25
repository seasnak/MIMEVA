using Godot;
using Mimeva;
using System;

public partial class Flag : Area2D
{
    [Export] private string final_scene_path = "";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {

    }

    private void OnBodyEntered(Node2D other)
    {
        if (other == null) { return; }

        if (other is Player)
        {
            // GD.Print("Player Finished Level!");

            try
            {
                GetTree().ChangeSceneToFile(final_scene_path);
            }
            catch
            {
                GD.PrintErr($"Error: Could not load scene \"{final_scene_path}\"");
                throw;
            }
        }


    }

}
