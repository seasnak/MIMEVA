using Godot;
using System;
using System.Collections.Generic;

namespace Mimeva;
/*
Loads in levels from custom .txt file format (see examples in pcglevels folder)
*/
public partial class BlockPlacer : Node2D
{

	private static Dictionary<int, string> level_dict; // dictionary representation of level
	private TileMap tilemap;
	private Player player;
	private string tilemap_path = "/root/world/Tilemap";
	private const int BLOCK_SIZE = 16; // size of each block

	private bool is_unix = true;

	// Prefabs Dictionary
	private Dictionary<string, PackedScene> block_dict;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ReloadBlockDictionary();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void ReloadBlockDictionary() {
		string[] block_type_list = {"player", "spikeball", "enemy", "flying_enemy", "door", "key", "coin", "checkpoint"};

		UpdateBlockDict("player", "res://Prefabs/Player.tscn");
		UpdateBlockDict("spikeball", "res://Prefabs/Objects/Spikeball.tscn");
		UpdateBlockDict("enemy", "res://Prefabs/Glorp.tscn");
		UpdateBlockDict("door", "res://Prefabs/Objects/door.tscn");
		UpdateBlockDict("key", "res://Prefabs/Objects/key.tscn");
		UpdateBlockDict("coin", "res://Prefabs/Objects/coin.tscn");
		UpdateBlockDict("checkpoint", "res://Prefabs/Objects/checkpoint.tscn");
		
	}

	private void UpdateBlockDict(string key, string val_path) {
		if(block_dict.ContainsKey(key)) {
			block_dict[key] = GD.Load<PackedScene>(val_path);
		}
		else {
			block_dict.Add(key, GD.Load<PackedScene>(val_path));
		}
	}

	public void LoadRoomFromFile(string target_f) {
		// Load in Levels and line up based on where we have left off
		
	}

	public void LoadPartFromFile(string part) {
		// Load in a part of a level (based on what part we need)

	}

	public void PlaceBlock(string block_name, Godot.Vector2 pos) {
		
	}
}
