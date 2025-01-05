using Godot;

using System;
using System.Collections.Generic;
using System.IO;

using Mimeva;
using Mimeva.Utils;

namespace Mimvea.Utils;

public partial class LevelUtils
{

    // PCG Functions
    public void ModifyLevel(ref Level level, int prev_diff, int new_diff)
    {

        // get the player death dictionary
        System.Collections.Generic.Dictionary<int, int> player_deaths_dict = LevelGenVariables.PlayerDeathDict;
        (int, int) target = (0, 0);
        Random rng = new();

        foreach (int key in player_deaths_dict.Keys)
        {
            if (player_deaths_dict[key] > target.Item2)
            {
                target = (key, player_deaths_dict[key]);
            }
            else if (player_deaths_dict[key] == target.Item2 && rng.NextInt64(0, 100) > 50)
            {
                target = (key, player_deaths_dict[key]);
            }
        }
        GD.Print($"max deaths to {target.Item1}: {target.Item2}");

    }
}
