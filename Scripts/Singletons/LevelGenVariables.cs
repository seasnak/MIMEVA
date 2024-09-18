using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Godot;

namespace Mimeva;

public partial class LevelGenVariables : Node
{
    // Constants
    // TODO: add a series of constants used to calculate + update difficulty
    // private static const Godot.Collections.Dictionary<int, Godot.Range> death_count_chart;

    // Variables
    private static float level_difficulty = 5f; // the level difficulty used to generate levels
    private static int num_rooms_completed = 0; // the number of rooms the player has completed in the current level
    private static int num_levels_completed = 0; // the number of levels the player has completed in total
    private static int player_death_count = 0; // the number of player deaths to determine level difficulty
    private static List<(float, float)> player_death_pos_list = new(); // list of places player has died

    // Getters and Setters

    public static float LevelDifficulty { get => level_difficulty; set => level_difficulty = value; }
	public static int NumRoomsCompleted { get => num_rooms_completed; set => num_rooms_completed = value; }
	public static int NumLevelsCompleted { get => num_levels_completed; set => num_levels_completed = value; }
    
    // Statistical Analysis Functions

    public static void AddNewDeath((float, float) pos) { // updates player death count and adds location of where player died
        player_death_count += 1;
        player_death_pos_list.Add(pos);
    }

    public static void AddNewDeath(ref Player player) {
        player_death_count += 1;
        player_death_pos_list.Add((player.Position.X, player.Position.Y));
    }

    public static void CalculateNewDifficulty() {
        
    }

    public static void DisplayPlayerDeaths() {

    }

}
