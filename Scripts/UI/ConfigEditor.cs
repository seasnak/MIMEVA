using Godot;
using System;

namespace Mimeva.Settings;
public partial class ConfigEditor : Label
{

    // TODO: Add some sort of dictionary or dictionary-like structure to store input values
    // private System.Collections.Generic.Dictionary<string, string[]> controls_dict = new();

    private string[] actions = new string[] { "Left", "Up", "Down", "Right", "interact", "attack", "jump", "dash", "skip" };
    [Export]
    private string[] menu_items = new string[] { "Left", "Up", "Down", "Right", "interact", "attack", "jump", "dash", "skip", "back" };

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {

    }

    private string GetMenuList(int pos)
    {
        string output = "";

        int i = 0;
        foreach (string item in menu_items)
        {


            output += "\n";
            i++;
        }
        return output;
    }

    private void EditBinding(string action, InputEvent new_keybind)
    {
        StringName sn_action = new StringName(action);
        Godot.Collections.Array<InputEvent> inputs = InputMap.ActionGetEvents(sn_action);
        InputMap.ActionEraseEvents(sn_action);

        // TODO: check to see if input is keyboard or controller input
        InputMap.ActionAddEvent(sn_action, new_keybind);
    }

    private void WriteConfig()
    {

    }

    private void GetConfigFromFile()
    {

    }

    private void GetConfigFromInputMap()
    {

    }

}
