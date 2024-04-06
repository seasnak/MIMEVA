using Godot;
using System;

namespace Mimeva;
public partial class Sign : Area2D
{

	[Export] private string[] text = {"Insert Dialogue Here"};

	// Sign Conditionals
	private bool player_interacted = false;

	// Nodes
	private DialogueManager dmanager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dmanager = GetNode<DialogueManager>("/root/DialogueManager");

		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(player_interacted) {
			dmanager.StartDialogue(this.GlobalPosition, text);
			player_interacted = false;

			GD.Print(text); // DEBUG: print out message
		}
	}

	private void OnBodyEntered(Node other) {
		if(other is not Player || other == null) { return; }
		
		if(Input.IsActionPressed("interact")) {
			GD.Print("Player Interacted");
			player_interacted = true;
		}
	}
}
