using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Mimeva;
public partial class PlayerVariables : Node
{


	private static Checkpoint curr_checkpoint = null;
	private static string checkpoint_scene_path = null;
	private static Node checkpoint_scene = null;

	private static Godot.Vector2 player_starting_pos;

	private static float level_difficulty = 5f; // the level difficulty used to generate levels
	private static int num_rooms_completed = 0; // the number of rooms the player has completed in the current level
	private static int num_levels_completed = 0; // the number of levels the player has completed in total
	private static int num_deaths = 0; // the number of player deaths

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// To do move to a different file at some point
		if (Input.IsKeyPressed(Godot.Key.Escape)) {
			GetTree().Quit();
		}
	}

	// public static void UpdatePlayerStat(string stat_name, float val) {
	// 	if(player_stats.ContainsKey(stat_name)) {
	// 		player_stats[stat_name] = val;
	// 	}
	// 	else {
	// 		player_stats.Add(stat_name, val);
	// 	}
	// }

	// Getters and Setters

	public static int NumDeaths { get => num_deaths; set => num_deaths = value; }
	public static Checkpoint GetCheckpoint() {
		return curr_checkpoint;
	}
    public static void SetCheckpoint(Checkpoint new_checkpoint) {
		curr_checkpoint = new_checkpoint;
	}

	public static Godot.Vector2 GetCheckpointPos() {
		return curr_checkpoint.GlobalPosition;
	}

	public static Node GetCheckpointScene() {
		return checkpoint_scene;
	}
	public static void SetChekpointScene(Node scene) {
		checkpoint_scene = scene;
	}

	public static string GetCheckpointScenePath() {
		return checkpoint_scene_path;
	}
	public static void SetCheckpointScenePath(string scene_path) {
		checkpoint_scene_path = scene_path;
	}

	public static Godot.Vector2 GetPlayerStartingPos() {
		return player_starting_pos;
	}
	public static void SetPlayerStartingPos(Godot.Vector2 pos) {
		player_starting_pos = pos;
	}

	public static void SetLevelDifficulty(float diff) {
		level_difficulty = diff;
	}
	public static float GetLevelDifficulty() {
		return level_difficulty;
	}
	public static float LevelDifficulty { get => level_difficulty; set => level_difficulty = value; }
	
	public static int NumRoomsCompleted { get => num_rooms_completed; set => num_rooms_completed = value; }

	public static int NumLevelsCompleted { get => num_levels_completed; set => num_levels_completed = value; }

}
