using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Godot;

namespace Mimeva.Utils;

public partial class Level {

    private List<List<string>> layout;
    private int in_pos;
    private int out_pos;
    private int in_size;
    private int out_size;

    // Getters Setters
    public List<List<string>> Layout { get => layout; set => SetLayout(ref value); }
    public int InPos { get => in_pos; set => in_pos = value; }
    public int OutPos { get => out_pos; set => out_pos = value; }
    public int InSize { get => in_size; set => in_size = value; }
    public int OutSize { get => out_size; set => out_size = value; }


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
            switch(line.ToLower()[..3]) { // compare first 3 characters
                case "":
                    break;
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

		s_writer.WriteLine($"{this.layout[0].Count} {this.layout.Count}\n\n");
		
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
        string level_str = "";



        return level_str;
    }




}