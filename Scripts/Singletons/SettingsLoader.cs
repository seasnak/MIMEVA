using Godot;
using System;

using Mimeva.Utils;
using Mimeva.UI;

namespace Mimeva.Settings;
public partial class SettingsLoader : Node2D
{

    // Settings File Variables
    private string settings_path = "res://settings.cfg";
    private ConfigFile cf;

    public override void _Ready()
    {
        cf = new();

        Error err = cf.Load(settings_path);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Could not load settings file at '{settings_path}'");
        }
        else
        {
            SetInputMapToConfig();
        }

        // DEBUG
        // SetConfigToInputMap();
    }

    private void ReloadSettings()
    {

    }

    private void AssignInputMap(string action, string input, bool is_keyboard = true)
    {
        if (InputMap.GetActions().Contains(action))
        {
            try
            {
                InputMap.ActionEraseEvents(action);
                InputEventKey ev = new();
                ev.PhysicalKeycode = OS.FindKeycodeFromString(input);
                InputMap.ActionAddEvent(action, ev);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error assigning \'{input}\' to {action}: {e}");
            }
        }
        else
        {
            GD.PrintErr($"Error, Action \'{action}\' does not exist.\nList of possible actions: Left, Right, Up, Down, Jump, Interact, Dash, Attack");
        }
    }

    private void SetInputMapToConfig()
    {
        // Sets values found in ConfigFile <cf> to InputMap (effectively rebind controls)
        foreach (string key in cf.GetSectionKeys("Input"))
        {
            Variant value = cf.GetValue("Input", key);
            string input_str = value.ToString().Split(' ')[0];
            GD.Print($"[Input] {key} : {input_str}"); // DEBUG

            AssignInputMap(key, input_str);
            // InputMap.ActionEraseEvent();

        }
        // StartGame.UpdateText();
    }

    private void SetConfigToInputMap()
    {
        Godot.Collections.Array<StringName> action_arr = InputMap.GetActions();
        foreach (string action in action_arr)
        {
            if (action.Contains("ui_")) { continue; } // skip unused inputs
            foreach (InputEvent input in InputMap.ActionGetEvents(action))
            {
                if (input.AsText().Split(' ')[0] == "Joypad") { continue; }
                string in_str = input.AsText();
                GD.Print($"{action}: {in_str}");
                cf.SetValue("Input", action, in_str);
            }
        }
        cf.Save(settings_path);
    }
}
