using Godot;
using System;

namespace Mimeva;
public partial class Sign : Area2D
{

	[Export] private string text = "";

	// Sign Conditionals
	private bool player_interacted;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(player_interacted) {

			player_interacted = false;
			//print out message
			GD.Print(text);
		}
	}

	private void OnBodyEntered(Node other) {
		if(other is not Player || other == null) { return; }
		
		player_interacted = true;
	}
}
