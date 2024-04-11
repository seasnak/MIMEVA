using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mimeva;
public partial class PlayerVariables : Node
{


	private static Checkpoint checkpoint = null;
	private static string checkpoint_scene_path = null;
	private static Node checkpoint_scene = null;

	private static Dictionary<string, float> player_stats;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player_stats = new();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public static void UpdatePlayerStat(string stat_name, float val) {
		if(player_stats.ContainsKey(stat_name)) {
			player_stats[stat_name] = val;
		}
		else {
			player_stats.Add(stat_name, val);
		}
	}

	// Getters and Setters
	public static Checkpoint GetCheckpoint() {
		return checkpoint;
	}

	public static Vector2 GetCheckpointPos() {
		return checkpoint.GlobalPosition;
	}

	public static void SetCheckpoint(Checkpoint new_checkpoint) {
		checkpoint = new_checkpoint;
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

}
