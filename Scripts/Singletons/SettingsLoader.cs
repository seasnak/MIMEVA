using Godot;
using System;
using System.Linq;

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
        // GD.Print(err);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Could not load settings file at '{settings_path}'");
            SetConfigToInputMap();
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

    private void AssignInputMap(string action, Godot.Collections.Array<string> inputs)
    {
        if (InputMap.GetActions().Contains(action))
        {
            try
            {
                InputMap.ActionEraseEvents(action);
                InputEventKey evkb = new();
                GD.Print("ActionMap: ", inputs[0], " -- ", OS.FindKeycodeFromString(inputs[0].Split(' ')[0]));
                evkb.PhysicalKeycode = OS.FindKeycodeFromString(inputs[0].Split(' ')[0]);
                InputMap.ActionAddEvent(action, evkb);

                // InputEventJoypadButton evjp = new();
                // InputMap.ActionAddEvent(action, evjp);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error assigning \'{inputs}\' to {action}: {e}");
            }
        }
        else
        {
            GD.PrintErr($"Error, Action \'{action}\' does not exist.\nList of possible actions: Left, Right, Up, Down, jump, interact, dash, attack, skip");
        }
    }

    private void SetInputMapToConfig()
    {
        // Sets values found in ConfigFile <cf> to InputMap (effectively rebind controls)
        foreach (string key in cf.GetSectionKeys("Input"))
        {
            Godot.Collections.Array<string> value = cf.GetValue("Input", key).AsGodotArray<string>();

            AssignInputMap(key, value);
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
        Godot.Collections.Array<string> input_arr = cf.GetValue("Input", action).AsGodotArray<string>();
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
