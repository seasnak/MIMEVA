using Godot;
/*using System;*/

public partial class StatsController : Control
{

    [Export] private Label death_label; // label containing death count
    [Export] private Label coin_label; // label containing number of coins collected
    [Export] private Label score_label; // label containing player's score at end of level

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        death_label = GetNode<Label>("Label");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
