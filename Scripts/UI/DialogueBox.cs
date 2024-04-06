using Godot;
using System;
using System.Text.RegularExpressions;

namespace Mimeva;

public partial class DialogueBox : MarginContainer
{

	// Properties
	private string text = "";
	private int let_idx = 0; // the current letter
	private float let_time = 0.03f; // the time it takes each letter to appear
	private float space_time = 0.06f; // the time it takes for spaces to appear
	private float punct_time = 0.2f; // the time it takes for punctuation to appear

	// Children Nodes
	private Label label;
	private Timer timer;

	// Constants
	private const int MAX_WIDTH = 256;

	// Signals
	private Signal FinishedDisplayingSignal;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("MarginContainer/Label");
		timer = GetNode<Timer>("LetterDisplayTimer");

		timer.Timeout += OnLetterDisplayTimerTimeout;
		this.Resized += OnResize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void DisplayText(string disp_text) {
		this.text = disp_text;
		label.Text = disp_text;
		
		// await resized
		// this.CustomMinimumSize = new Vector2(Math.Min(Size.X, MAX_WIDTH), CustomMinimumSize.Y);
		
		if (Size.X > MAX_WIDTH) {
			label.AutowrapMode = TextServer.AutowrapMode.Word;
			// await resized
			// await resized
			CustomMinimumSize = new Vector2(CustomMinimumSize.X, Size.Y);
		}

		GlobalPosition -= new Vector2(Size.X / 2, Size.Y + 24);
		label.Text = "";
	}

	private void DisplayLetter() {
		label.Text += text[let_idx];
		
		let_idx += 1;
		if (let_idx >= text.Length) {
			// emit FinishedDisplayingSignal
			return;
		}

		switch(text[let_idx]) {
			case '!': case '.': case ',': case '?':
				timer.Start(punct_time);
				break;
			case ' ':
				timer.Start(space_time);
				break;
			default: 
				timer.Start(let_time);
				break;
		}
	}

	private void OnLetterDisplayTimerTimeout() {
		DisplayLetter();
	}
}
