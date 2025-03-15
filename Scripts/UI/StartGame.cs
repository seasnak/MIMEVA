using Godot;
using System;

namespace Mimeva.UI;
public partial class StartGame : Label
{
    [Export] private string starting_scene_path = "";
    [Export] private string text_suffix = "to Start";
    [Export] private string tr_action = "jump"; // action that triggers the scene change event
    [Export] private bool is_blinking = true; // if true, then the text will fade in and out
    private float curr_alpha = 0;

    // Getters and Setters
    public string StartingScenePath { get => starting_scene_path; set => starting_scene_path = value; }
    public string TextSuffix { get => text_suffix; set => text_suffix = value; }
    public string TrAction { get => tr_action; set => tr_action = value; }
    public bool IsBlinking { get => is_blinking; set => is_blinking = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Text = $"Press  '{InputMap.ActionGetEvents(tr_action)[0].AsText().Split(' ')[0]}' ({tr_action}) {text_suffix}";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        curr_alpha += 2 * (float)delta;

        if (is_blinking) { this.Modulate = new Godot.Color(this.Modulate.R, this.Modulate.G, this.Modulate.B, (1 + (float)Math.Sin(curr_alpha)) / 2); }

        if (Input.IsActionJustPressed(tr_action))
        {
            try
            {
                GetTree().ChangeSceneToFile(starting_scene_path);
            }
            catch
            {
                GD.PrintErr("error loading scene!");
                GetTree().Quit();
            }
        }
    }

    public void UpdateText()
    {
        this.Text = $"Press  '{InputMap.ActionGetEvents(tr_action)[0].AsText().Split(' ')[0]}' ({tr_action}) {text_suffix}";
    }
}
