using Godot;
/*using System;*/
using Mimeva;

namespace Mimeva.UI;

public partial class StatsController : Control
{

    [Export] private Label stats_label;

    private string stats_header = "===   POINTS   ===\n";
    private string final_stats_template = $"deaths:{PlayerVariables.NumDeaths}\n";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stats_label = GetNode<Label>("Stats");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
