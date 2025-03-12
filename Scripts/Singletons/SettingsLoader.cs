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

    private Godot.Collections.Dictionary JOYPAD_BUTTONS = new()
    {

    };

    public override void _Ready()
    {
        cf = new();
        Error err = cf.Load(settings_path);

        // if (err != Error.Ok)
        // {
        //     GD.PrintErr($"Could not load settings file at '{settings_path}'");
        // }
        // else
        // {
        //     SetInputMapToConfig();
        // }

        // DEBUG
        SetConfigToInputMap();
    }

    private void ReloadSettings()
    {
    }

    private void AssignInputMap(string action, string[] inputs)
    {
        if (InputMap.GetActions().Contains(action))
        {
            try
            {
                InputMap.ActionEraseEvents(action);
                InputEventKey evkb = new();
                evkb.PhysicalKeycode = OS.FindKeycodeFromString(inputs[0]);
                InputMap.ActionAddEvent(action, evkb);

                InputEventJoypadButton evjp = new();
                // evjp.ButtonIndex = ...;

                InputMap.ActionAddEvent(action, evjp);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error assigning \'{inputs}\' to {action}: {e}");
            }
        }
        else
        {
            GD.PrintErr($"Error, Action \'{action}\' does not exist.\nList of possible actions: Left, Right, Up, Down, Jump, Interact, Dash, Attack, Skip");
        }
    }

    private void SetInputMapToConfig()
    {
        // Sets values found in ConfigFile <cf> to InputMap (effectively rebind controls)
        foreach (string key in cf.GetSectionKeys("Input"))
        {
            string[] input_arr = (string[])cf.GetValue("Input", key);
            // Variant value = cf.GetValue("Input", key);
            // string input_str = value.ToString().Split(' ')[0];
            // GD.Print($"[Input] {key} : {input_str}"); // DEBUG

            AssignInputMap(key, input_arr);
        }
        // StartGame.UpdateText();
    }

    private void SetConfigToInputMap()
    {
        Godot.Collections.Array<StringName> action_arr = InputMap.GetActions();
        foreach (string action in action_arr)
        {
            if (action.Contains("ui_")) { continue; } // skip unused inputs

            string[] input_arr = new string[2]; // input_arr[keyboard_input_str, controller_input_str]
            foreach (InputEvent input in InputMap.ActionGetEvents(action))
            {
                string in_str = input.AsText();
                GD.Print($"{action}: {in_str}");
                if (in_str.Split(' ')[0] == "Joypad") { input_arr[1] = in_str; }
                else { input_arr[0] = in_str; }
            }
            cf.SetValue("Input", action, input_arr);
        }
        cf.Save(settings_path);
    }

    private void SetConfigToInputEvent(string action, InputEvent input)
    {   // updates a single action <action> to input (InputEvent) <input> in config file
        string[] input_arr = (string[])cf.GetValue("Input", action);
        if (input is InputEventKey) { input_arr[0] = input.AsText(); }
        else { input_arr[1] = input.AsText(); }
        cf.SetValue("Input", action, input_arr);
    }

    public void UpdateConfig(string action, string input, bool is_keyboard = true)
    {
        // AssignInputMap(action, input, is_keyboard);
        string[] input_arr = (string[])cf.GetValue("Input", action);
        if (is_keyboard)
        {
            input_arr[0] = input;
        }
        else
        {
            input_arr[1] = input;
        }
        AssignInputMap(action, input_arr);
        cf.SetValue("Input", action, input_arr);
    }
}
