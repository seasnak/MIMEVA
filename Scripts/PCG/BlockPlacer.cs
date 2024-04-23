using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mimeva;
/*
Loads in levels from custom .txt file format (see examples in pcglevels folder)
*/
public partial class BlockPlacer : Area2D
{

	// private static Dictionary<int, string> level_dict; // dictionary representation of level
	private List<List<string>> level_mat;
	private TileMap tilemap;
	private Player player;
	private string tilemap_path = "/root/world/Tilemap";
	private string level_folder = "res://Levels";
	
	// Constants 
	private const int BLOCK_SIZE = 8; // size of each tilemap block in pixels
	
	// Tilemape to World Coordinates Scale
	private const int T2W_SCALE = 8;
	private const int T2W_OFFSET = 4;
	
	[Export] private Godot.Vector2 curr_offset = Godot.Vector2.Zero; // offset for the next room
	private Godot.Vector2 left_connector_pos = Godot.Vector2.Zero; // the location of the left connector
	private Godot.Vector2 right_connector_pos = Godot.Vector2.Zero; // the location of the right connector
	private bool is_unix = true;

	private bool tmp_generating_level = false;

	// Prefabs Dictionary
	private Godot.Collections.Dictionary<string, PackedScene> block_dict;
	

	// Enums
	enum RoomPart { Left, Middle, Right };
	// enum Object { Block, Player, Spikeball, Enemy, FlyingEnemy, Door, Key, Coin, Checkpoint };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		block_dict = new();
		ReloadBlockDictionary();

		tilemap = GetNode<TileMap>("/root/World/TileMap"); // get tilemap node
		
		// default offsets based on starting level
		curr_offset = new(8, 0);
		right_connector_pos = new(8, 2);

		// add signals
		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void ReloadBlockDictionary() {
		// string[] block_type_list = {"player", "spikeball", "enemy", "flying_enemy", "door", "key", "coin", "checkpoint"};

		UpdateBlockDict("P", "res://Prefabs/Player.tscn");
		UpdateBlockDict("S", "res://Prefabs/Objects/Spikeball.tscn");
		UpdateBlockDict("E", "res://Prefabs/Glorp.tscn");
		UpdateBlockDict("D", "res://Prefabs/Objects/door.tscn");
		UpdateBlockDict("K", "res://Prefabs/Objects/key.tscn");
		UpdateBlockDict("*", "res://Prefabs/Objects/coin.tscn");
		UpdateBlockDict("C", "res://Prefabs/Objects/checkpoint.tscn");
		
	}

	public void UpdateBlockDict(string key, string val_path) {
		if(block_dict.ContainsKey(key)) {
			block_dict[key] = ResourceLoader.Load<PackedScene>(val_path);
		}
		else {
			block_dict.Add(key, ResourceLoader.Load<PackedScene>(val_path));
		}
	}

	public void LoadRoomFromFile(string target_f) {
		// Load in Levels and line up based on where we have left off
		
	}

	public void LoadNewRoomFromPartFiles(int start_pos_x=-1, int start_pos_y=-1) {
		// Loads in Levels using Room Parts and lines it up based on where the previous room ended

		// Todo: change to be a random room once more parts are generated
		// res://Levels/Parts/Left/LE1_10.txt
		LoadPartFromFile($"{level_folder}/Parts/Left/LE1_10.txt");
		LoadPartFromFile($"{level_folder}/Parts/Middle/ME1_10.txt");
		LoadPartFromFile($"{level_folder}/Parts/Right/RE1_10.txt");
		
	}

	public void LoadPartFromFile(string part_f, int start_pos_x = -1, int start_pos_y = -1) {
		// Load in a part of a level (based on what part we need)
		if(start_pos_x < 0) { 
			start_pos_x = (int)curr_offset.X; 
			start_pos_y = (int)curr_offset.Y;
		}
		Godot.Vector2 start_pos = new(start_pos_x, start_pos_y);

		GetLevelMatFromFile(part_f); // update level_mat to correct output
		BuildLevelFromLevelMat(); // 
		
	}

	private void BuildLevelFromLevelMat() {
		// loop through the level matrix
		for(int i = 0; i < level_mat.Count; i++) {
			for(int j = 0; j < level_mat[0].Count; j++) {
				if(level_mat[i][j] == "B") { // case: block
					tilemap.SetCellsTerrainConnect(0, new Godot.Collections.Array<Vector2I> {new Vector2I(i + (int)curr_offset.X, j + (int)curr_offset.Y)}, 0, 0);
				}
				else { // case: not a block, so an object
					Vector2 obj_pos = new((i + (int)curr_offset.X)*BLOCK_SIZE, (j + (int)curr_offset.Y)*BLOCK_SIZE);

					if(block_dict.ContainsKey(level_mat[i][j])) {
						var obj = block_dict[level_mat[i][j]].Instantiate();
						((Node2D)obj).Position = obj_pos;
					}
					else {
						GD.Print($"Error: Block {level_mat[i][j]} not in block dictionary.");
					}
				}
			}
		}
	}

	private void SaveLevelToFile(string outfile) {
		// saves level to <outfile>

		GD.Print("Saving Level");
		Console.WriteLine("Saving Level");
		for(int i = 0; i<level_mat.Count; i++) {
			for(int j = 0; j<level_mat[0].Count; j++) {
				
			}
		}
		

	}

	private void PlaceObject(string obj_name, Godot.Vector2 pos) {
		//Place an object of type <obj_name> at position <pos>
		var obj = block_dict[obj_name].Instantiate();

		((Node2D)obj).GlobalPosition = pos;
	}

	private void GetLevelMatFromFile(string level_f) {

		// string[,] level_arr = {};
		level_mat = new(); // clear level matrix
		
		// check to see if file exists
		if (Godot.FileAccess.FileExists(level_f)) {
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

		PrintRoom();
		return;
	}

	private void PrintRoom() {
		GD.Print(level_mat);
	}

	// Signal Functions
	private void OnBodyEntered(Node other) {
		if(other is not Player || other == null) { return; }

		GD.Print("Player Entered!");
		if(!tmp_generating_level) {
			tmp_generating_level = true;
			LoadNewRoomFromPartFiles();
		}
	}
}
