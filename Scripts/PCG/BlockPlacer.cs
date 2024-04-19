using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mimeva;
/*
Loads in levels from custom .txt file format (see examples in pcglevels folder)
*/
public partial class BlockPlacer : Node2D
{

	// private static Dictionary<int, string> level_dict; // dictionary representation of level
	private List<List<string>> level_mat;
	private TileMap tilemap;
	private Player player;
	private string tilemap_path = "/root/world/Tilemap";
	private const int BLOCK_SIZE = 12; // size of each block
	private Godot.Vector2 curr_offset = Godot.Vector2.Zero; // offset for the next room
	private bool is_unix = true;

	// Prefabs Dictionary
	private Godot.Collections.Dictionary<Object, PackedScene> block_dict;

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

	public void LoadPartFromFile(string part_f, Godot.Vector2 start_pos) {
		// Load in a part of a level (based on what part we need)
		if(start_pos == null) { start_pos = curr_offset; }

		GetLevelMatFromFile(part_f, ref level_mat);
		BuildLevel(ref level_mat);

	}

	private void BuildLevel(ref List<List<string>> level) {

	}

	private void PlaceObject(Object obj_name, Godot.Vector2 pos) {
		//Place an object of type <obj_name> at position <pos>
		var obj = block_dict[obj_name].Instantiate();

		((Node2D)obj).GlobalPosition = pos;
	}

	private static void GetLevelMatFromFile(string level_f, ref List<List<string>> level_mat) {

        // string[,] level_arr = {};
		level_mat = new(); // clear level matrix
		
		// check to see if file exists
		if (!File.Exists(level_f)) {
			GD.Print($"File {level_f} does not exist!");
			return;
		}
		
		// load level from file
		int l = -1; // current line number in file
		int d_line = 0;
		foreach (string line in File.ReadLines(level_f)) {
			l++;
			string[] contents = line.Split(' ');

			if(l == 0) { // initialize array
				level_mat = new List<List<string>>(contents[0].ToInt());
				for(int i=0; i<contents[1].ToInt(); i++) {
					level_mat.Add(new List<string>(contents[1].ToInt()));
				}

			}
			if(contents[0] == "ROOM") {
				if (d_line > contents[0].ToInt()) { continue; } 
				for (int x = 0; x < contents.Length; x++) {
					level_mat[d_line][x] = contents[x];
				}
				d_line++;
			}
		}

		PrintRoom(level_mat);
		return;
	}

	private static void PrintRoom(List<List<string>> level_mat) {
		GD.Print(level_mat);
	}
}
