using System;
using Godot;

/*using Mimeva;*/
namespace Mimeva.PCG;

public partial class LevelGen : Node
{

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {

    }

    public void GenerateNewLevel(int difficulty = 5, Vector2? shape = null)
    {
        if (shape == null) { shape = new(10, 10); }

    }

    public void ModifyExistingLevel(int target_difficulty, string fname)
    {
        char start_difficulty = fname[0]; // difficulty of level from file --> E = 3, M = 5, H = 8
        int diff_change = 0;
        switch (start_difficulty)
        {
            case 'E': diff_change = target_difficulty - 3; return;
            case 'M': diff_change = target_difficulty - 5; return;
            case 'H': diff_change = target_difficulty - 8; return;
            default: diff_change = 0; return;
        };
    }

    /*private string[] beats = { "^ - -", "^ _ _", "^ _ -", "_ ^ _", "- _ -" };*/

    public string GenerateRhythm(int num_measures = 2, int difficulty = 5, string mode = "regular", int measure_length = 4, bool repeat_measure = true, bool has_upbeat = false)
    {
        // increased difficulty decreases the space between notes or introduces "difficulty" patterns with uneven spacing
        // rhythm consists of "emphasized" jumps (^) and "muted" jumps (-) and rests (_)
        string rhythm = "";
        Random rand = new();

        if (mode == "regular") // notes are evenly spaced
        {
            if (has_upbeat) rhythm += "- ";
            int rhythm_gen_len = num_measures * measure_length; // the number of notes to generate
            if (repeat_measure) { rhythm_gen_len = measure_length; }

            for (int i = 0; i < rhythm_gen_len; i++)
            {
                if (i % measure_length == 0) { rhythm += "^ "; continue; } // first beat of measure is always the only "emphasized" jump

                float rand_int = rand.NextSingle();
                if (rand_int < 0.6) { rhythm += "- "; }
                else { rhythm += "_ "; }
            }

            // repeate measure x times
            if (repeat_measure) { for (int i = 0; i < num_measures; i++) rhythm += rhythm; }
        }
        else if (mode == "swing") // notes have a "swing" beat to them
        {

        }
        else
        {
            GD.Print("Invalid rhythm mode.");
        }

        return rhythm;
    }

}
