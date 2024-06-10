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

    private List<List<string>> layout;
    private int in_pos;
    private int out_pos;
    private int in_size;
    private int out_size;
    private int[] shape = new int[2];

    private readonly Dictionary<string, string> level_key;

    // Getters Setters
    public List<List<string>> Layout { get => layout; set => SetLayout(ref value); }
    public int InPos { get => in_pos; set => in_pos = value; }
    public int OutPos { get => out_pos; set => out_pos = value; }
    public int InSize { get => in_size; set => in_size = value; }
    public int OutSize { get => out_size; set => out_size = value; }

    public int[] Shape { get => shape; set => shape = value; }

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

        // // get total shape of output level
        // foreach(string infile in infiles) {
        //     using StreamReader sr = new(infile);
        //     while(!sr.EndOfStream) {
        //         output = sr.ReadLine();
        //         if(output[..5] == "SHAPE") {
        //             total_shape[0] += output.Split(' ')[1].ToInt();
        //             total_shape[1] += output.Split(' ')[2].ToInt();
        //             break;
        //         }
        //     }
        // }
        
        string[] output_arr;
        int level_part_rows = 0;

        foreach(string infile in infiles) {
            using StreamReader sr = new(infile);
            while(!sr.EndOfStream) {

                output_arr = sr.ReadLine().Split(' ');
                if(output_arr[0][..2] == "//") { continue; } // comment line so skip -- checking first for efficiency
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
OUTPUS 3

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