using Godot;
using System;
using System.Text.RegularExpressions;

namespace Mimeva.UI;

public partial class DialogueBox : MarginContainer
{

	// Properties
	private string text = "";
	private int let_idx = 0; // the current letter
	private float let_time = 0.01f; // the time it takes each letter to appear
	private float space_time = 0.02f; // the time it takes for spaces to appear
	private float punct_time = 0.01f; // the time it takes for punctuation to appear
	private float disappear_time = 1f; 

	// Children Nodes
	private Label label;
	private Timer timer;

	// Constants
	private const int MAX_WIDTH = 512;
	
	// Signals
	private static readonly string DialogueFinishedSignal = "DialogueFinished";

	// Booleans
	private bool is_line_finished = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("MarginContainer/Label");
		timer = GetNode<Timer>("LetterDisplayTimer");

		timer.Timeout += OnLetterDisplayTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void DisplayText(string disp_text) {
		// disp_text += "\0";
		is_line_finished = false;
		this.text = disp_text;
		label.Text = disp_text;
		
		// resize dialogue text box
		this.Size = new Vector2(Size.X, CustomMinimumSize.Y);
		if (Size.X > MAX_WIDTH) {
			label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			label.CustomMinimumSize = new(MAX_WIDTH, label.Size.Y);
			this.Size = new(MAX_WIDTH, 48*(int)(Size.X/MAX_WIDTH));
			// this.Size = new(MAX_WIDTH, this.Size.Y);
		}
		GlobalPosition = new(GlobalPosition.X - (Size.X / 8), GlobalPosition.Y - (this.Size.Y/4) - 10);
		label.Text = "";
		DisplayLetter();
	}

	private void DisplayLetter() {
		label.Text += text[let_idx];
		
		let_idx += 1;

		if (let_idx >= text.Length) {
			is_line_finished = true;
			// this.QueueFree();
			return;
		}

		switch(text[let_idx]) {
			case '!': case '.': case ',': case '?':
				timer.Start(punct_time);
				break;
			case ' ':
				timer.Start(space_time);
				break;
			case '\0':
				timer.Start(disappear_time);
				break;
			default: 
				timer.Start(let_time);
				break;
		}
	}

	private void OnLetterDisplayTimerTimeout() {
		// add a delay to text print speed in dialogue box
		DisplayLetter();
		return;
	}

	public static string GetDialogueFinishedSignal() { return DialogueFinishedSignal; }

	public bool GetIsLineFinished() { return is_line_finished; }
}
