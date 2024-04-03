using Godot;
using System;
using System.Reflection.Metadata;

namespace Mimeva;
public partial class DialogueManager : Node
{

	private PackedScene dbox_scene;

	private string[] dbox_lines_arr = Array.Empty<string>();
	private int curr_line_idx = 0;
	
	private DialogueBox dbox;
	private Godot.Vector2 dbox_pos = new();

	private bool is_dialogue_active = false;
	private bool can_advance_line = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dbox_scene = GetNode<PackedScene>("res://Prefabs/UI/DialogueBox.tscn");


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void StartDialogue(Godot.Vector2 pos, string[] lines) {
		if(is_dialogue_active){ return; }

		dbox_lines_arr = lines;
		dbox_pos = pos;
		ShowTextbox();

		is_dialogue_active = true;
	}

	private void ShowTextbox() {
		dbox = (DialogueBox) dbox_scene.Instantiate();
		
		GetTree().Root.AddChild(dbox);
		dbox.GlobalPosition = dbox_pos;
		dbox.DisplayText(dbox_lines_arr[curr_line_idx]);
	}
	
	
}
