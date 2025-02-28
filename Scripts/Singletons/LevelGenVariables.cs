using System;
using System.Collections.Generic;
using Godot;

using Mimeva.Entity;
using Mimeva.Utils;

namespace Mimeva.PCG;
public partial class LevelGenVariables : Node
{
    // Level Generation Specific Variables
    private static float level_difficulty = 5f; // the level difficulty used to generate levels
    private static int player_death_tolerance = 5; // the tolerance of how many times a player can die before difficulty is increased
    private static float difficulty_change_scaling = 0.2f;

    private static int num_parts_completed = 0; // the number of parts the player has completed in the current level
    private static int num_rooms_completed = 0; // the number of rooms the player has completed in total
    private static int num_rooms_generated = 0; // the number of rooms generated (in case player skips)
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
    public static int NumRoomsGenerated { get => num_rooms_generated; set => num_rooms_generated = value; }
    public static int NumPartsCompleted { get => num_parts_completed; set => num_parts_completed = value; }
    public static Dictionary<int, int> PlayerDeathDict { get => player_death_dict; }
    public static int PlayerDeathCount { get => player_death_count; }
    public static int NumRooms { get => num_rooms; set => num_rooms = value; }

    // Constants
    private const int BLOCK_SIZE = 8; // size of each tilemap block in pixels
    private const int BLOCK_OFFSET = 4; // offset to place blocks at

    // Files
    private static string death_filename = "res://upload_this_death_data.dat";
    private static string level_filename = "res://upload_this_level_data.dat";

    // Statistical Analysis Functions
    public static void AddRoomToLevelDict(string level, Vector2 shape)
    {
        string content = "";
        if (!level_gen_hist_dict.ContainsKey(num_rooms_generated))
        {
            level_gen_hist_dict.Add(num_rooms_generated, new List<string>());
            content += $"====== Room {num_rooms_generated} ======\n";
        }
        level_gen_hist_dict[num_rooms_generated].Add(level);
        int last_idx = level_gen_hist_dict[num_rooms_generated].Count - 1;
        // GD.Print(num_rooms_generated + ": " + level_gen_hist_dict[num_rooms_completed][last_idx]);

        content += $"{level} -- ({shape})\n";
        FileUtils.SaveToTxt(filename: level_filename, content: content);
    }

    public static void AddNewDeath((float, float) pos, String object_name = null)
    { // updates player death count and adds location of where player died
        AddNewDeath(new Vector2(pos.Item1, pos.Item2), object_name);
    }

    public static void AddNewDeath(ref Player player, String object_name = null)
    {
        AddNewDeath(player.GlobalPosition, object_name);
    }

    public static void AddNewDeath(Godot.Vector2 player_pos, String object_name = null)
    {
        player_death_count += 1;
        player_death_pos_list.Add((player_pos.X, player_pos.Y));
        Godot.Vector2 grid_pos = (player_pos / BLOCK_SIZE) - new Godot.Vector2(BLOCK_OFFSET, BLOCK_OFFSET); // convert the physical position on the plane to the cell location

        string content = "";
        content += $"GlobalPosition: {player_pos}\n";
        content += $"GridPosition: {grid_pos}\n";
        content += $"RoomNumber: {num_rooms_generated}\n";
        content += $"Difficulty: {level_difficulty}";
        content += $"\n";

        GD.Print(content);
        FileUtils.SaveToTxt(filename: death_filename, content: content);
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
