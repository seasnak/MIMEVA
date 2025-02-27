using System;
using System.Collections.Generic;
using Godot;

using Mimeva.Entity;

namespace Mimeva.PCG;
public partial class LevelGenVariables : Node
{
    // Level Generation Specific Variables
    private static float level_difficulty = 5f; // the level difficulty used to generate levels
    private static int player_death_tolerance = 5; // the tolerance of how many times a player can die before difficulty is increased
    private static float difficulty_change_scaling = 0.2f;

    private static int num_parts_completed = 0; // the number of parts the player has completed in the current level
    private static int num_rooms_completed = 0; // the number of rooms the player has completed in total
    private static int num_rooms = 3; // the number of levels to generate
    private static int num_rooms_per_level = 5; // the minimum number of rooms in each level
    private static int player_death_count = 0; // the number of player deaths to determine level difficulty
    private static List<(float, float)> player_death_pos_list = new(); // list of places player has died

    // booleans
    private static bool player_has_skipped = false;
    public static bool PlayerHasSkipped { get => player_has_skipped; set => player_has_skipped = value; }
    private static Dictionary<int, int> player_death_dict = new(); // dictionary containing player deaths per certain difficulty

    // Globals
    public static Dictionary<int, List<string>> level_gen_hist_dict = new(); // dictionary containing all generated levels

    // Getters and Setters
    public static int PlayerDeathTolerance { get => player_death_tolerance; set => player_death_tolerance = value; }
    public static float DifficultyChangeScaling { get => difficulty_change_scaling; set => difficulty_change_scaling = value; }
    public static float LevelDifficulty { get => level_difficulty; set => level_difficulty = value; }
    public static int NumRoomsCompleted { get => num_rooms_completed; set => num_rooms_completed = value; }
    public static int NumPartsCompleted { get => num_parts_completed; set => num_parts_completed = value; }
    public static Dictionary<int, int> PlayerDeathDict { get => player_death_dict; }
    public static int PlayerDeathCount { get => player_death_count; }
    public static int NumRooms { get => num_rooms; set => num_rooms = value; }

    // Statistical Analysis Functions
    public static void AddNewDeath((float, float) pos)
    { // updates player death count and adds location of where player died
        player_death_count += 1;
        player_death_pos_list.Add(pos);

        if (!player_death_dict.ContainsKey((int)Math.Round(level_difficulty, 0)))
        {
            player_death_dict.Add((int)Math.Round(level_difficulty, 0), 0);
        }
        player_death_dict[(int)Math.Round(level_difficulty, 0)] += 1;
    }

    public static void AddRoomToLevelDict(string level)
    {
        if (!level_gen_hist_dict.ContainsKey(num_rooms_completed))
        {
            level_gen_hist_dict.Add(num_rooms_completed, new List<string>());
        }
        level_gen_hist_dict[num_rooms_completed].Add(level);
        int last_idx = level_gen_hist_dict[num_rooms_completed].Count - 1;
        GD.Print(num_rooms_completed + ": " + level_gen_hist_dict[num_rooms_completed][last_idx]);
    }

    public static void AddNewDeath(ref Player player)
    {
        player_death_count += 1;
        player_death_pos_list.Add((player.Position.X, player.Position.Y));

        player_death_dict[(int)Math.Round(level_difficulty, 0)] += 1;
    }

    public static void AddNewDeath(Godot.Vector2 pos)
    {
        player_death_count += 1;
        player_death_pos_list.Add((pos.X, pos.Y));

        player_death_dict[(int)Math.Round(level_difficulty, 0)] += 1;
    }

    public static float UpdateDifficulty()
    {
        if (num_rooms_completed == 0) return -1f;

        //
        float new_diff = level_difficulty + (1 / (player_death_count + 1));
        GD.Print($"Difficulty Update: {level_difficulty} -> {new_diff}");
        level_difficulty = Math.Min(10, new_diff);

        level_difficulty = new_diff;
        return new_diff;
    }

    public static void DisplayPlayerDeaths()
    {

    }

}
