using Godot;
using System;

using Mimeva.UI;

namespace Mimeva.Utils;
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
        dbox_scene = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/DialogueBox.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    public void StartDialogue(Godot.Vector2 pos, string[] lines)
    {
        if (is_dialogue_active) { return; } // don't start a new dialogue if a dialogue is currently active
        if (lines.Length <= 0) { GD.Print("No lines passed to Dialogue Manager"); return; } // empty lines array

        dbox_lines_arr = lines;
        // GD.Print(dbox_lines_arr); // DEBUG: print out dbox lines array
        dbox_pos = pos;
        ShowTextbox();

        is_dialogue_active = true;
    }

    private void ShowTextbox()
    {
        dbox = (DialogueBox)dbox_scene.Instantiate();
        // dbox.FinishedDisplaying.Connect(OnTextBoxFinishedDisplaying); // FIX THIS
        GetTree().Root.AddChild(dbox);
        dbox.GlobalPosition = dbox_pos;
        dbox.DisplayText(dbox_lines_arr[curr_line_idx]);
    }

    public void AdvanceDialogue()
    {
        dbox.QueueFree();

        curr_line_idx += 1;
        if (curr_line_idx >= dbox_lines_arr.Length)
        { // dialogue box is done
            is_dialogue_active = false;
            curr_line_idx = 0;
            return;
        }

        ShowTextbox();
    }

    public void KillDBox()
    {
        if (dbox == null) { return; }

        is_dialogue_active = false;
        curr_line_idx = 0;
        try
        {
            dbox.QueueFree();
        }
        catch
        {
            GD.Print("dbox already freed");
        }
    }

    public bool GetDialogueBoxFinishedLine()
    {
        if (dbox == null) { return false; }
        return dbox.GetIsLineFinished();
    }
    public bool GetDialogueManagerDisplayingLines() { return can_advance_line; }


}
