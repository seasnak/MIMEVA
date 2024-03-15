using Godot;
using System;
using System.Collections.Generic;

namespace Mimeva;
/*
Loads in levels from custom .txt file format (see examples in pcglevels folder)
*/
public partial class LevelLoader : Node2D
{

	private static Dictionary<int, string> level_dict; // dictionary representation of level
	private TileMap tilemap;
	private Player player;
	private string tilemap_path = "/root/world/Tilemap";

	private bool is_unix = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void LoadTSCNFromFile(string target_f) {
		/* 
			Build a level from <target_f> .txt file
		*/

		tilemap.Clear();

		string target_fpath = "";
		if(is_unix) { target_fpath = $"Levels/{target_f}"; }
		else{ target_fpath = $"Levels\\{target_f}"; }
		GD.Print($"Opening File {target_fpath}");


		// create a dictionary to contain adjacent levels
		level_dict = new Dictionary<int, string>();

		int j_idx = 0; // hor_idx for line
		int i_lc = 0; // line count in file

		Vector2 dims = new Vector2(); // the dimension 
		foreach(string line in System.IO.File.ReadLines(target_fpath)) {
			i_lc++;
			GD.Print($"{line}");
			if(line.Length == 0) { continue; } // skip empty lines
			else if(i_lc == 1) { // first line contains level dimensions
				var dims_arr = line.Split(" ");
				dims = new Vector2(dims_arr[0].ToFloat(), dims_arr[1].ToFloat());
				continue;
			}
			else if( true ) { // todo: fix case: check level lines
				int level_symbol = line.Substring(1, 1).ToInt();
				string target_new_level = line[3..]; // get the level name for the level

				level_dict[level_symbol] = target_new_level; //save fp to dictionary
				continue;	
			}
		}
	}
}
