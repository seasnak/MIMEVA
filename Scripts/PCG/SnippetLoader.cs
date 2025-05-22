using Godot;
using System;
using Mimeva.Utils;

namespace Mimeva.PCG;
public partial class SnippetLoader : Node2D
{

    [Export] private string target_level_filepath = "";
    [Export] private Level snippet;

    public override void _Ready() { }

    public override void _Process(double delta) { }

}
