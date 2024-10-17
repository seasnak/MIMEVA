using Godot;
using Mimeva;
using System;

public partial class Flag : Area2D
{
    [Export] private string final_scene_path;

    public override void _Ready() {
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta) {

    }

    private void OnAreaEntered(Node2D other) {
        if(other == null) { return; }

        if(other is Player) {
            GD.Print("Player Finished Level!");
            GetTree().ChangeSceneToFile(final_scene_path);
        }

    }

}
