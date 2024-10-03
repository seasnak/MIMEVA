using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Godot;

namespace Mimeva.Utils;

// NOTE: use a LevelKey object to set a custom key

public partial class Level {

    // level details
    private List<List<string>> layout;
    private int in_pos; // the position of the opening for the entrance to the level (relative to the level and not to the world)
    private int out_pos; // the position of the opening for the exit to the level
    private int in_size; // the size of the opening for the entrance
    private int out_size; // the size of the opening for the exit
    private int[] shape = new int[2]; // the size of the level (x, y)

    // Block and Level-Building Related Variables
    private Dictionary<string, PackedScene> level_object_dict = new(); // dictionary that links the strings found in each level with their object representation. (ie. {"C":<Coin Prefab>} would place a coin wherever "C" appears in Level.layout)
    private string block_key = "B"; // default string that represents a tilemap block in the level
    private string empty_key = "-"; // default string that represents and empty tile in the level
    private TileMapLayer block_tilemap; // tilemap to construct the level out of
    private TileMapLayer fg_tilemap; // tilemap for the foreground elements 
    private TileMapLayer bg_tilemap; // tilemap for the background elements
    private int BLOCK_SIZE = 8; // the size of each block (in pixels) -- default: 8x8 pixel tiles
    private int BLOCK_OFFSET = 4; // the intial offset (in pixels) -- default: an offset of (4,4) pixels

    // Getters and Setters
    public List<List<string>> Layout { get => layout; set => SetLayout(ref value); }
    public int InPos { get => in_pos; set => in_pos = value; }
    public int OutPos { get => out_pos; set => out_pos = value; }
    public int InSize { get => in_size; set => in_size = value; }
    public int OutSize { get => out_size; set => out_size = value; }

    public int[] Shape { get => shape; set => shape = value; }

    public string BlockKey { get => block_key; set => block_key = value; }
    public string EmptyKey { get => empty_key; set => empty_key = value; }
    public TileMapLayer BlockTilemap { get => block_tilemap; set => block_tilemap = value; }

    public int BlockSize { get => BLOCK_SIZE; set => BLOCK_SIZE = value; }
    public int BlockOffset { get => BLOCK_OFFSET; set => BLOCK_OFFSET = value; }
    
    public Level() {
        in_pos = 0;
        out_pos = 0;
        in_size = -1;
        out_size = -1;
        
        layout = new List<List<string>>();
    }

    public Level(ref List<List<string>> _layout, int _in_size=-1, int _out_size=-1, int _in_pos=0, int _out_pos=0) {
        
        in_pos = _in_pos;
        out_pos = _out_pos;

        in_size = _in_size;
        out_size = _out_size;

        // deep copy level matrix
        SetLayout(ref _layout);
    }

    // -----------------------------------------------------
    // Level Object Setters and Getters
    // -----------------------------------------------------
    public void SetLayout(ref List<List<string>> _layout) {
        // creates a deep copy and sets it to layout field
        
        for(int i = 0; i < _layout.Count; i++) {
            for(int j = 0; j < _layout[0].Count; j++) {
                layout[i][j] = _layout[i][j];
            }
        }
    }


    public Level GetLevelFromTxt(string infile) {
        
        if(!File.Exists(infile)) {
            GD.PrintErr($"Error: File {infile} not found!");
            return new();
        }

        Level level = new();
        string line;
        using StreamReader sr = new(infile);
        
        while(!sr.EndOfStream) {
            line = sr.ReadLine();
            
            if(line.Length == 0) { continue; } // empty line
            else if(line[..2] == "//") { continue; } // comment line

            switch(line.ToLower()[..3]) { // compare first 3 characters
                case "":
                    break;
            }
        }

        return level;
    }

    public Level GetLevelFromTxtParts(string[] infiles, bool is_horizontal = true) {
        /*
        Generates a level consisting of all level part files passed in with <infiles>
        Note: all level parts must have the same x size (for vertically generated rooms), or same y size (for horizontally generated rooms)
        */
        Level level = new();
        int[] total_shape = new int[2]{0, 0};
        
        string[] output_arr;
        int level_part_rows = 0;

        foreach(string infile in infiles) {
            using StreamReader sr = new(infile);
            while(!sr.EndOfStream) {

                output_arr = sr.ReadLine().Split(' ');
                if(output_arr[0][..2] == "//") { continue; } // comment line so skip -- checking this first for efficiency
                switch(output_arr[0]) {
                    case "SHAPE": // update the shape of the level
                        level_part_rows = output_arr[1].ToInt();
                        level.shape = new int[2]{level.shape[0] + output_arr[1].ToInt(), level.shape[1] + output_arr[2].ToInt()};
                        break;
                    case "INSIZE":
                        if(level.out_size > 0 && level.out_size != output_arr[1].ToInt()) {
                            // error occurred -- mismatch on sizes
                            Console.Write("Error generating level using level parts -- mismatched level parts.");
                            throw new Exception("Level Part Mismatch"); // TODO: change to non-generic exception
                        }
                        break;
                    case "OUTSIZE":
                        level.out_size = output_arr[1].ToInt();
                        break;
                    case "INPOS":
                        level.in_pos = output_arr[1].ToInt();
                        break;
                    case "OUTPOS":
                        level.out_pos = output_arr[1].ToInt();
                        break;
                    case "ROOM":
                        int depth = 0;
                        output_arr = sr.ReadLine().Split(' ');
                        while(depth < level.shape[0] && output_arr.Length != 0) { // append room part to level
                            level.layout[depth].AddRange( output_arr.ToList() );
                            depth += 1;
                            output_arr = sr.ReadLine().Split(' ');
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        

        return level;
    }

    public Level GetLevelFromJSON(string infile) {
        Level level = new();
        
        return level;
    }

    public void SaveLevelToTxt(string outfile) {
		// saves level <level> to <outfile>
		using StreamWriter s_writer = new(outfile);

		s_writer.WriteLine($"SHAPE {this.layout[0].Count} {this.layout.Count}\n\n");
		
		s_writer.WriteLine("ROOM\n");
		string level_row = "";
		for(int i = 0; i<this.layout.Count; i++) {
			for(int j = 0; j<this.layout[0].Count; j++) {
				level_row += this.layout[i][j] + " ";
			}
			s_writer.WriteLine($"{level_row}\n");
		}

        s_writer.WriteLine($"\nINPOS {in_pos}\nOUTPOS {out_pos}\nINSIZE {in_size}\nOUTSIZE {out_size}\n");
	}

	public void SaveLevelToJSON(string outfile) {
		// save level <level> to a JSON file <outfile> 
	}

	public void ConvertLevelTxtToJSON(string in_txt, string out_json) {
        // converts a level txt file to a json level file
        using StreamReader s_reader = new(in_txt);
    }

	public void ConvertLevelJSONToText(string in_json, string out_txt) {
		// converts a json level file to a txt level file
		
	}

    // Functions to Generate Levels from Level Object in Godot
    
    public void BuildLevel(ref SceneTree world, Godot.Vector2? start_pos = null) {
        /*
        Constructs level in Godot
        <start_pos> is the offset at which to start building the level
        */
        if(start_pos == null) { start_pos = Godot.Vector2.Zero; }
        
        for(int i = 0; i < this.shape[0]; i++) {
            for(int j = 0; j < this.shape[1]; j++) {
                string val = this.layout[i][j];

                if(val == empty_key) { } // skip -- empty block
                else if(val == block_key) {
                    block_tilemap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> {new Vector2I(j + (int)((Godot.Vector2)start_pos).X, i + (int)((Godot.Vector2)start_pos).Y)}, 0, 0);
                }
                else if(val == "L") { // TODO: change to have a variable value for left connector
                       
                }
                else if(val == "R") { // TODO: change to have a variable value for right connector
                    
                }
                else {
                    if(level_object_dict.ContainsKey(val)) {
                        var obj = level_object_dict[val].Instantiate();
                        world.Root.CallDeferred("add_child", obj);
                        ((Node2D)obj).Position = new(
                            j + (int)((Godot.Vector2)start_pos).X*BLOCK_SIZE + BLOCK_OFFSET,
                            i + (int)((Godot.Vector2)start_pos).Y*BLOCK_SIZE + BLOCK_OFFSET
                        );
                    }
                    else { GD.Print($"Error: Block {val} is not in block dict."); }
                }
            }
        }

    }

    // -----------------------------------------------------
    // Functions to Manipulate Blocks and the BlockDictionary
    // -----------------------------------------------------
    public void UpdateLevelObjectDict(string key, string val_path) {
		if(level_object_dict.ContainsKey(key)) {
			level_object_dict[key] = ResourceLoader.Load<PackedScene>(val_path);
		}
		else {
			level_object_dict.Add(key, ResourceLoader.Load<PackedScene>(val_path));
		}
	}
    public void UpdateLevelObjectDict(string key, PackedScene scene) {
        if(level_object_dict.ContainsKey(key)) {
            level_object_dict[key] = scene;
        }
        else {
            level_object_dict.Add(key, scene);
        }
    }
    public void ClearLevelObjectDict() { level_object_dict.Clear(); }
    public void SetLevelObjectDict(ref Dictionary<string, PackedScene> bd) {
        level_object_dict.Clear();
        foreach (string key in bd.Keys) { level_object_dict.Add(key, bd[key]); }
    }
    public void AppendToLevelObjectDict(ref Dictionary<string, PackedScene> bd, bool overwrite_duplicate_keys = false) {
        /*
        Append all keys and values from <bd> to <Level.level_object_dict>
        If <overwrite_duplicate_keys> is false, keys that already exist in <Level.level_object_dict> are skipped
        */
        foreach (string key in bd.Keys) {
            if(level_object_dict.ContainsKey(key)) {
                if(overwrite_duplicate_keys) {
                    level_object_dict[key] = bd[key];
                }
            }
            else { 
                level_object_dict.Add(key, bd[key]); 
            }
        }
    }

    // Utility Functions
    public string PrintLevel() {
        // Prints out current level
        string level_str = "";
        
        for(int i = 0; i < this.layout[0].Count; i++) {
            for(int j = 0; j < this.layout.Count; j++) {
                level_str += this.layout[i][j] + " ";
            }
            level_str += "\n";
        }
           
        return level_str;
    }

    // ------------------------------------------------------------
    // Functions for Decorations and Background/Foreground Tilemaps
    // ------------------------------------------------------------
    private void DecorateLevel() {



    }
}


/* SAMPLE LEVEL TEXT FILE
// SHAPE -- contains the shape of the object
SHAPE 5 5

// KEY -- contains the connecting rooms (connections that don't exist can be ommited)
// ie. this key can be rewritten as:
// L level_1.txt
// R level_2.txt
//
KEY
L level_1.txt
R level_2.txt
U
D

// (optional) -- contains the input and output position of the room
INPOS 3
OUTPOS 3

// (optional) -- contains the input and output size of the room
INSIZE 5
OUTSIZE 5

ROOM
B B B B B
B - - - B
- - - - -
L - * - R
B B B B B

*/