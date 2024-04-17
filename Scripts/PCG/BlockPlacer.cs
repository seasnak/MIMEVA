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
	private const int BLOCK_SIZE = 12; // size of each block
	private Godot.Vector2 curr_offset = Godot.Vector2.Zero; // offset for the next room
	private bool is_unix = true;

	// Prefabs Dictionary
	private Dictionary<Object, PackedScene> block_dict;

	// Enums
	enum RoomPart { Left, Middle, Right };
	enum Object { Block, Player, Spikeball, Enemy, FlyingEnemy, Door, Key, Coin, Checkpoint };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ReloadBlockDictionary();

		tilemap = GetNode<TileMap>("/root/World/Tilemap");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void ReloadBlockDictionary() {
		// string[] block_type_list = {"player", "spikeball", "enemy", "flying_enemy", "door", "key", "coin", "checkpoint"};

		UpdateBlockDict(Object.Player, "res://Prefabs/Player.tscn");
		UpdateBlockDict(Object.Spikeball, "res://Prefabs/Objects/Spikeball.tscn");
		UpdateBlockDict(Object.Enemy, "res://Prefabs/Glorp.tscn");
		UpdateBlockDict(Object.Door, "res://Prefabs/Objects/door.tscn");
		UpdateBlockDict(Object.Key, "res://Prefabs/Objects/key.tscn");
		UpdateBlockDict(Object.Coin, "res://Prefabs/Objects/coin.tscn");
		UpdateBlockDict(Object.Checkpoint, "res://Prefabs/Objects/checkpoint.tscn");
		
	}

	private void UpdateBlockDict(Object key, string val_path) {
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

	private void PlaceObject(Object obj_name, Godot.Vector2 pos) {
		//Place an object of type <obj_name> at position <pos>
		var obj = block_dict[obj_name].Instantiate();
		
	}
}
