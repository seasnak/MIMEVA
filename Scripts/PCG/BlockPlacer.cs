using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace Mimeva;
/*
Loads in levels from custom .txt file format (see examples in pcglevels folder)
*/
public partial class BlockPlacer : Area2D
{
	private List<List<string>> level_mat; // list representation of level

	// Imported Nodes
	private TileMap tilemap;
	private Player player;
	

	// private string tilemap_path = "/root/world/TileMap"; // where the TileMap is located
	private string level_folder = "res://Levels";
	private string connector_room_path = "res://Levels/Rooms/C_10.txt";
	
	// Constants 
	private const int BLOCK_SIZE = 8; // size of each tilemap block in pixels
	private const int BLOCK_OFFSET = 4; // offset to place blocks at
	
	// Level Variables
	[Export] private Godot.Vector2 curr_offset = Godot.Vector2.Zero; // offset for the next room
	private Godot.Vector2 left_connector_pos = Godot.Vector2.Zero; // the location of the left connector
	private Godot.Vector2 right_connector_pos = Godot.Vector2.Zero; // the location of the right connector
	private bool is_unix = true;
	private bool tmp_generating_level = false; // temporary variable to ensure that the level isn't generated every time the player passes through
	private float difficulty = 5f; // difficulty between 1 and 10. determines how many room parts are of "easy", "medium", or "hard" difficulty.
	private int num_parts_in_room = 5; // the number of parts that will make up the room.

	// Prefabs Dictionary
	private Godot.Collections.Dictionary<string, PackedScene> block_dict;
	
	// levels dictionary
	private Godot.Collections.Dictionary<string, string[]> parts_dict;
	// private System.Collections.Generic.Dictionary<string, string[]> level_dict;

	// enums	
	private enum Difficulty {EASY, MEDIUM, HARD};
	private enum Part {LEFT, MIDDLE, RIGHT};

	private readonly string[] diff_arr = {"Easy", "Medium", "Hard"};
	private readonly string[] part_arr = {"Left", "Middle", "Right"};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// play animations
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");
		
		// initialize empty dictionaries
		block_dict = new();
		parts_dict = new();

		ReloadBlockDictionary();
		ReloadLevelPartsDictionary(); // loads in the list of levels

		tilemap = GetNode<TileMap>("/root/World/TileMap"); // get tilemap node
		
		// default offsets based on starting level
		curr_offset = new(8, -5);
		right_connector_pos = new(8, -5);

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

	public void ReloadLevelPartsDictionary(string path="res://Levels/Parts/") {
		// adds and sorts all level parts from 
		string[] files = System.Array.Empty<string>();
		string global_path = ProjectSettings.GlobalizePath(path);
		
		foreach (string part in part_arr) {
			global_path = ProjectSettings.GlobalizePath(path + part + "/");

			foreach (string diff in diff_arr) {
				// difficulties are EASY MEDIUM HARD
				string key = $"{part}{diff}";
				GD.Print($"searching {global_path} for {part}{diff} levels");
				
				files = Directory.GetFiles(global_path, $"*{part[0]}{diff[0]}*", SearchOption.AllDirectories);
				
				// add files to dictionary
				if(parts_dict.ContainsKey(key)) {
					parts_dict[key] = files;
				}
				else {
					parts_dict.Add(key, files);
				}
			}
		}
		
		GD.Print(parts_dict); // DEBUG
		
	}

	public void LoadRoomFromFile(string target_f) {
		// Load in Levels and line up based on where we have left off
		GetLevelMatFromFile(target_f);
		BuildLevelFromLevelMat();
	}

	private string GetNewDifficulty() {
		Random random = new();
		string diff = "Hard";
		if(difficulty <= 3) { diff = "Easy"; }
		else if(difficulty == 4 ) { 
			diff = random.NextDouble() > 0.5 ? "Medium" : "Easy"; 
		}
		else if(difficulty <= 7) { diff = "Medium"; }
		else if(difficulty == 8) {
			diff = random.NextDouble() > 0.5 ? "Hard" : "Medium";
		}
		return diff;
	}

	public void LoadNewRoomFromPartFiles(int start_pos_x=-1, int start_pos_y=-1, int num_parts=-1) {
		// Loads in Levels using Room Parts and lines it up based on where the previous room ended
		Random random = new();

		// select a random amount of parts for the level
		if(num_parts == -1) {
			// select the minimum amount of rooms based on the current difficulty settin
			int min_parts = 3;
			int max_parts = (int)Math.Floor(difficulty/3) + 3;
			num_parts_in_room = random.Next(min_parts, max_parts+1); 
		} 
		else { 
			num_parts_in_room = num_parts; 
		}

		// randomly pick room parts from parts dictionary
		string diff_str = GetNewDifficulty();
		int curr_parts_len = parts_dict["Left"+diff_str].Length;
		LoadPartFromFile($"{ parts_dict["Left"+diff_str][random.Next(0, curr_parts_len)] }"); // load left part

		for(int i=0; i<num_parts_in_room; i++) {
			//load in middle room parts
			diff_str = GetNewDifficulty();
			curr_parts_len = parts_dict["Middle"+diff_str].Length;
			LoadPartFromFile($"{parts_dict["Middle"+diff_str][random.Next(0, curr_parts_len)]}");		
		}

		diff_str = GetNewDifficulty();
		curr_parts_len = parts_dict["Right"+diff_str].Length;
		LoadPartFromFile($"{ parts_dict["Right"+diff_str][random.Next(0, curr_parts_len)] }");

		GD.Print($"Loading in {ProjectSettings.GlobalizePath(connector_room_path)}");
		LoadPartFromFile(ProjectSettings.GlobalizePath(connector_room_path));

	}

	public void LoadPartFromFile(string part_f, int start_pos_x = -1, int start_pos_y = -1) {
		// Load in a part of a level (based on what part we need)
		if(start_pos_x < 0) {
			start_pos_x = (int)curr_offset.X; 
			start_pos_y = (int)curr_offset.Y;
		}
		Godot.Vector2 start_pos = new(start_pos_x, start_pos_y);

		GetLevelMatFromFile(part_f); // update level_mat to correct output
		BuildLevelFromLevelMat(); // build the level given the current matrix
		
	}

	private void BuildLevelFromLevelMat() {
		// loop through the level matrix
		for(int i = 0; i < level_mat.Count; i++) {
			for(int j = 0; j < level_mat[0].Count; j++) {
				if(level_mat[i][j] == "B") {
					tilemap.SetCellsTerrainConnect(0, new Godot.Collections.Array<Vector2I> {new Vector2I(j + (int)curr_offset.X, i + (int)curr_offset.Y)}, 0, 0);
				}
				else {
					Vector2 obj_pos = new((j + (int)curr_offset.X)*BLOCK_SIZE + BLOCK_OFFSET, (i + (int)curr_offset.Y)*BLOCK_SIZE + BLOCK_OFFSET);
					
					try { block_dict.ContainsKey(level_mat[i][j]); }
					catch(Exception e) { GD.Print($"Error {e}: could not find key {level_mat[i][j]}"); }

					if(block_dict.ContainsKey(level_mat[i][j])) {
						var obj = block_dict[level_mat[i][j]].Instantiate();
						// GetTree().Root.AddChild(obj);
						GetTree().Root.CallDeferred("add_child", obj);
						((Node2D)obj).Position = obj_pos;
					}
					else if(level_mat[i][j] == "-") { continue; } // empty element
					else if(level_mat[i][j] == "L") { // update left connector position
						left_connector_pos = new Godot.Vector2(i, j) + curr_offset;
					}
					else if(level_mat[i][j] == "R") { // update right connector position
						right_connector_pos = new Godot.Vector2(i, j) + curr_offset;
					}
					else if(level_mat[i][j] == "!") { // move block placer here
						this.Position = obj_pos;
						tmp_generating_level = false;
					}
					else {
						GD.Print($"Error: Block {level_mat[i][j]} not in block dictionary.");
					}
				}
			}
		}

		//update current offset
		if(level_mat.Count == 0) { return; }
		curr_offset += new Godot.Vector2(level_mat[0].Count, 0);
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
		if (ResourceLoader.Exists(level_f)) {
			GD.Print($"file \"{level_f}\" does not exist, or could not be found.");
			return;
		}
		
		// load level from file
		int l = -1; // current line number in file
		int room_y = 0; // line in the room or part of room

		bool in_room_contents = false; // if true, then currently reading through the room text representation

		using var file = Godot.FileAccess.Open(level_f, Godot.FileAccess.ModeFlags.Read);
		string lines = file.GetAsText();
		foreach (string line in lines.Split('\n')) {
			l++; // increment line counter

			if(line.Length == 0) { continue; } // empty line so skip
			
			string[] contents = line.Split(' ');

			if(l == 0) { // initialize array
				GD.Print($"creating matrix of size ({contents[0]},{contents[1]})");
				level_mat = new List<List<string>>(contents[0].ToInt());
				for(int i=0; i<contents[0].ToInt(); i++) {
					level_mat.Add( new List<string>(contents[1].ToInt()) );
				}
				GD.Print(level_mat.Count, " ", level_mat[0].Count);
				
			}
			else if(line == "ROOM") {
				in_room_contents = true;
			}
			else if(in_room_contents) {
				for (int x = 0; x < contents.Length; x++) { // read through line
					level_mat[room_y].Add(contents[x]);
				}
				room_y++;
			}
		}

		PrintRoom(); // DEBUG
		
		return;
	}

	private void PrintRoom() {
		string line;
		foreach(var list in level_mat) {
			line = "";
			foreach(var obj in list) {
				line += obj + " ";
			}
			GD.Print(line);
		}
	}

	// Signal Functions
	private void OnBodyEntered(Node other) {
		if(other is not Player || other == null) { return; }

		// GD.Print("Player Entered!");
		if(!tmp_generating_level) {
			tmp_generating_level = true;
			LoadNewRoomFromPartFiles();
		}
	}

	private void UpdateTilemap(string new_tilemap_path) {
		try {
			tilemap = GetNode<TileMap>(new_tilemap_path);
		}
		catch(Exception e) {
			GD.Print($"Error {e}: Could not change tilemap. \"{new_tilemap_path}\" does not exist, or is not a valid tilemap");	
		}
	}

	private void UpdateTileset(string new_tileset_path) {
		try{
			tilemap.TileSet = ResourceLoader.Load<TileSet>(new_tileset_path);
		}
		catch(Exception e) {
			GD.Print($"Error {e}: Could not cange tileset. \"{new_tileset_path}\" does not exist or is not a valid tileset");
		}
	}
}
