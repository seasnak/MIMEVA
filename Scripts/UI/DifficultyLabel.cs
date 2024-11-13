using Godot;
using Mimeva;
using System;
using Mimeva;

namespace Mimeva.UI;

public partial class DifficultyLabel : RichTextLabel {

	Player player;
	

	public override void _Ready() {
		this.Text = "Difficulty:   ";
    
    // Adjust Vars
    this.Position = new(10, 20);
	}

	public override void _Process(double delta) {
		this.Text = $"Difficulty:   LevelGenVariables.LevelDifficulty";
	}
}
