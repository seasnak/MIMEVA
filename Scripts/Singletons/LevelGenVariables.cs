using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Godot;

namespace Mimeva;

public partial class LevelGenVariables : Node
{
    private static float level_difficulty = 5f; // the level difficulty used to generate levels
    private static int num_rooms_completed = 0; // the number of rooms the player has completed in the current level
    private static int num_levels_completed = 0; // the number of levels the player has completed in total

    public static float LevelDifficulty { get => level_difficulty; set => level_difficulty = value; }
	
	public static int NumRoomsCompleted { get => num_rooms_completed; set => num_rooms_completed = value; }

	public static int NumLevelsCompleted { get => num_levels_completed; set => num_levels_completed = value; }
}
