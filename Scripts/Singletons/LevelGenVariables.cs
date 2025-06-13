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

    private static List<int> player_total_kill_count = new(); // the total amount of enemies that player has killed at the end of each level
    private static List<int> player_kill_count = new(); // the amount of enemies that player kills in each level
    private static List<double> level_start_time = new(); // the time at which a player starts a certain level part
    private static List<double> level_complete_time = new(); // the calculated time it takes for a player to complete a level part

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

    public static List<int> PlayerKillCount { get => player_kill_count; }
    public static List<double> LevelStartTime { get => level_start_time; }


    // Constants
    private const int BLOCK_SIZE = 8; // size of each tilemap block in pixels
    private const int BLOCK_OFFSET = 4; // offset to place blocks at

    // Files
    private static string death_filename = "res://upload_this_death_data.dat";
    private static string level_filename = "res://upload_this_level_data.dat";

    public override void _Ready()
    {
        // update data lists
        level_start_time.Add(Time.GetTicksMsec());
    }

    // Statistical Analysis Functions
    public static void AddRoomToLevelDict(string level, Vector2 shape)
    {
        string content = "";
        if (!level_gen_hist_dict.ContainsKey(num_rooms_generated))
        {
            level_gen_hist_dict.Add(num_rooms_generated, new List<string>());
            if (num_rooms_generated == 0) content += "===========================================";
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
        if (num_rooms_completed <= 1) return -1f; // skip initial "tutorial" room

        GD.Print("Updating difficulty", num_rooms_completed);

        // initial level difficulty adjustment
        float new_diff = level_difficulty + (float)(2 / (player_death_count + 1));

        // adjust difficulty according to completion time
        PrintList(level_complete_time);

        if (level_complete_time[num_rooms_completed - 2] > 90000)
        { // case: player has taken longer than 90 seconds to complete a level
            new_diff -= Math.Min(2, (float)(level_complete_time[num_rooms_completed - 2] / 60000) / player_death_count);
        }
        else if (level_complete_time[num_rooms_completed - 2] < 30000)
        {
            new_diff += (float)(15000 / level_complete_time[num_rooms_completed - 2]) * 0.33f;
        }

        // adjust difficulty according to enemy kill rate
        // PrintList(player_kill_count
        new_diff += player_kill_count[num_rooms_completed - 2] * 0.2f;

        // adjust difficulty according to coin collection rate
        //

        // adjust difficulty according to notable death rate
        if (player_death_count >= 5)
        {
            new_diff -= player_death_count * 0.2f;
        }

        GD.Print($"Difficulty Update: {level_difficulty} -> {new_diff}");
        level_difficulty = (float)Math.Round(Math.Max(1, Math.Min(10, new_diff)), 2);

        return level_difficulty;
    }

    public static void UpdatePlayerStats()
    {
        // called once at each new blockplacer
        // updates all of the player related stats such as time taken to complete level and number of enemies killed
        if (num_rooms_completed == 0) return;

        // update count for number of enemies player killed in last room
        player_total_kill_count.Add(PlayerVariables.EnemyKillCount);
        if (player_kill_count.ToArray().Length <= num_rooms_completed)
        {
            player_kill_count.Add(0);
        }

        // update
        level_start_time.Add(Time.GetTicksMsec());
        level_complete_time.Add(level_start_time[num_rooms_completed] - level_start_time[num_rooms_completed - 1]);
    }

    public static void UpdatePlayerKillCount(int num_to_add)
    {
        if (player_kill_count.ToArray().Length <= num_rooms_completed)
        {
            player_kill_count.Add(num_to_add);
        }
        else
        {
            player_kill_count[num_rooms_completed] += num_to_add;
        }
        return;
    }

    public static void PrintList<T>(List<T> a)
    {
        string output = "[";

        foreach (var val in a)
        {
            output += val + ", ";
        }

        output += "]";
        GD.Print(output);
    }

}
