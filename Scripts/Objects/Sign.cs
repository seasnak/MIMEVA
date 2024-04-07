using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Mimeva;
public partial class Sign : Area2D
{

	[Export] private string[] text = {"Insert Dialogue Here"};

	// Sign Conditionals
	private bool player_inside = false;
	private bool is_displaying = false;

	// Nodes
	private DialogueManager dmanager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dmanager = GetNode<DialogueManager>("/root/DialogueManager");

		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if(player_inside && Input.IsActionJustPressed("interact") && !is_displaying) {
			// player interacted with sign, so display dialogue
			is_displaying = true;
			dmanager.StartDialogue(this.GlobalPosition, text);
			DialogueBox dbox = dmanager.GetDialogueBox();
			
			dbox.QueueFree();
			is_displaying = false;
		}
	}

	private void OnBodyEntered(Node other) {
		if(other is not Player || other == null) { return; }
		
		player_inside = true;
	}

	private void OnBodyExited(Node other) {
		if(other is not Player || other == null) { return; }
		
		player_inside = false;
	}
}
