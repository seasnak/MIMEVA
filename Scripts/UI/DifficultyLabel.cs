using Godot;
using Mimeva;
using System;

namespace Mimeva.UI;

public partial class DifficultyLabel : RichTextLabel {

	Player player;	

	public override void _Ready() {
    this.Text = "Difficulty:   ";
    
    // Adjust Appearance
    this.Position = new(10, 20); 
    this.Size = new(100, 100);
    this.ScrollActive = false;
	}

	public override void _Process(double delta) {
		this.Text = $"Current Difficulty: {LevelGenVariables.LevelDifficulty}";
	}
}
