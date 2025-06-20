using Godot;

using System;
using System.Collections.Generic;
using System.IO;

using Mimeva.Utils;
using Mimeva.Entity;
using Mimeva.Object;

namespace Mimeva.PCG;

public partial class BlockPlacer : Area2D
{
    private List<List<string>> level_mat; // list representation of level

    // Imported Nodes
    private TileMapLayer tilemap;
    private Player player;
    public void SetTilemap(string nodepath) { tilemap = GetNode<TileMapLayer>(nodepath); }
    public void SetPlayer(string nodepath) { player = GetNode<Player>(nodepath); }


    // Important Objects
    private Flag flag;

    // Level Folder/Filepaths
    private string level_folder_path = "res://Levels";
    private string connector_room_path = "res://Levels/Rooms/C_10.txt";
    private string final_room_path = "res://Levels/Rooms/X_10.txt";

    // Constants
    private const int BLOCK_SIZE = 8;
    public int BLOCK_SIZE_C { get => BLOCK_SIZE; } // getter
    private const int BLOCK_OFFSET = 4;
    public int BLOCK_OFFSET_C { get => BLOCK_OFFSET; } // getter

    // Level Variables
    [Export] private Godot.Vector2 curr_offset = Godot.Vector2.Zero; // offset for the next room
    public void SetOffset(Vector2 offset) { curr_offset = offset; right_connector_pos = offset; }
    private Godot.Vector2 left_connector_pos = Godot.Vector2.Zero; // the location of the left connector
    private Godot.Vector2 right_connector_pos = Godot.Vector2.Zero; // the location of the right connector

    [Export] private float override_difficulty = 0; // if the override difficulty is between 1 and 10, then override difficulty to this value
    [Export] private int num_parts_in_room = 5; // the number of parts that will make up the room.
    [Export] private int num_rooms_to_generate = 3; // UNUSED the number of rooms to generate before ending the level
    private int num_rooms_generated = 0; // UNUSED (counter) the number of rooms generated so far (depracated -- use LevelGenVariables.NumRoomsCompleted instead)

    // Booleans
    private bool is_generating_level = false; // temporary variable to ensure that the level isn't generated every time the player passes through
    private bool place_excess = false; // replaces excess Os with spikes to give the illusion that a level is harder than it actually is

    // Level Modifiers
    private float excess_spike_rate = 0.5f; // the rate at which to convert Os into spikes
    private int num_keys = 0;

    // Booleans for level generation
    private bool is_key_room = false;
    private bool key_has_been_placed = false;

    // Prefabs Dictionary
    private static Godot.Collections.Dictionary<string, PackedScene> block_dict = new();

    // Level parts dictionary
    private static Godot.Collections.Dictionary<string, string[]> snippet_dict;

    // Arrays for difficulty level difficulty and part files
    private readonly string[] SNIPPET_DIFFICULTIES = { "Easy", "Medium", "Hard" };
    private readonly string[] SNIPPET_TYPES = { "Left", "Middle", "Right" };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        // play animations
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");

        // initialize empty dictionaries
        block_dict = new();
        snippet_dict = new();

        ReloadBlockDictionary();
        ReloadLevelPartsDictionary(); // loads in the list of levels

        // get tilemap node
        tilemap = GetNode<TileMapLayer>("/root/World/TileMap/Platforms");

        // get player node
        player = GetNode<Player>("/root/World/Player");

        // default offsets based on starting level
        curr_offset = new(8, -5);
        right_connector_pos = new(8, -5);

        // add signals
        BodyEntered += OnBodyEntered;

        // set difficulty
        if (override_difficulty > 0) { LevelGenVariables.LevelDifficulty = override_difficulty; }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("skip") && !LevelGenVariables.PlayerHasSkipped)
        {

            if (LevelGenVariables.NumRoomsCompleted >= 1)
            {
                LevelGenVariables.LevelDifficulty = Math.Max(1, LevelGenVariables.LevelDifficulty - 1 - LevelGenVariables.PlayerDeathCount / 10);
            }

            if (LevelGenVariables.NumRoomsGenerated == LevelGenVariables.NumRooms)
            {
                LevelGenVariables.PlayerHasSkipped = true;
                player.GlobalPosition = flag.GlobalPosition - new Vector2(15, 5);
            }
            else
            {
                LevelGenVariables.NumRoomsCompleted = Math.Max(0, LevelGenVariables.NumRoomsCompleted - 1);
                LevelGenVariables.PlayerHasSkipped = true;
                player.GlobalPosition = this.GlobalPosition - new Vector2(15, 5); // move player to this position
            }
        }
    }

    // Loads all prefabs into the Block Dictionary
    public void ReloadBlockDictionary()
    {
        UpdateBlockDict("P", "res://Prefabs/Player.tscn");
        UpdateBlockDict("S", "res://Prefabs/Objects/Spikeball.tscn");
        UpdateBlockDict("GL", "res://Prefabs/Enemies/Glorp.tscn");
        UpdateBlockDict("FL", "res://Prefabs/Enemies/Fleep.tscn");
        UpdateBlockDict("D", "res://Prefabs/Objects/door.tscn");
        UpdateBlockDict("K", "res://Prefabs/Objects/key.tscn");

        UpdateBlockDict("*", "res://Prefabs/Objects/coin.tscn");
        UpdateBlockDict("C", "res://Prefabs/Objects/checkpoint.tscn");
        UpdateBlockDict("F", "res://Prefabs/Objects/flag.tscn");
    }

    public static void UpdateBlockDict(string key, string val_path)
    {
        if (block_dict.ContainsKey(key))
        {
            block_dict[key] = ResourceLoader.Load<PackedScene>(val_path);
        }
        else
        {
            block_dict.Add(key, ResourceLoader.Load<PackedScene>(val_path));
        }
    }

    public void ReloadLevelPartsDictionary(string path = "res://Levels/Parts/")
    {
        // adds and sorts all level parts from
        string[] files = System.Array.Empty<string>();
        string global_path = ProjectSettings.GlobalizePath(path);

        foreach (string snippet in SNIPPET_TYPES)
        {
            global_path = ProjectSettings.GlobalizePath(path + snippet + "/");

            foreach (string difficulty in SNIPPET_DIFFICULTIES)
            {
                // difficulties are EASY MEDIUM HARD
                string key = $"{snippet}{difficulty}";

                files = Directory.GetFiles(global_path, $"*{snippet[0]}{difficulty[0]}*", SearchOption.AllDirectories);

                // add files to dictionary
                if (snippet_dict.ContainsKey(key))
                {
                    snippet_dict[key] = files;
                }
                else
                {
                    snippet_dict.Add(key, files);
                }
            }
        }
    }

    public void LoadRoomFromFile(string target_f)
    {
        // Load in Levels and line up based on where we have left off
        GetLevelMatFromTxtFile(target_f);
        BuildLevelFromLevelMat();
    }


    // Gets the difficulty of the next level
    private string GetNewDifficulty()
    {
        Random random = new();
        string diff;

        if (LevelGenVariables.LevelDifficulty <= 5)
        {
            diff = random.NextDouble() < (LevelGenVariables.LevelDifficulty / 10) ? "Easy" : "Medium";
        }
        else if (LevelGenVariables.LevelDifficulty < 10)
        {
            diff = random.NextDouble() < (LevelGenVariables.LevelDifficulty / 10) - 0.5 ? "Medium" : "Hard";
        }
        else { diff = "Hard"; }

        return diff;
    }

    public void LoadNewRoomFromPartFiles(int start_pos_x = -1, int start_pos_y = -1, int num_parts = -1)
    {
        // Load in Level using <num_parts> Snippets at (<start_pos_x>, <start_pos_y>)
        Random random = new();

        if (num_parts == -1)
        {
            // select the minimum amount of rooms based on the current difficulty setting
            int min_parts = 3;
            int max_parts = (int)Math.Floor((float)LevelGenVariables.LevelDifficulty / 3) + 3;
            num_parts_in_room = random.Next(min_parts, max_parts + 1);
        }
        else
        {
            num_parts_in_room = num_parts;
        }

        // Generate Rhythm of length <num_parts>
        int measure_len = random.Next(3, 6);
        string rhythm = new LevelGen().GenerateRhythm(num_measures: num_parts_in_room / measure_len, measure_length: measure_len);
        GD.Print(rhythm);

        // randomly pick room parts from parts dictionary
        int curr_parts_len;
        string diff_str = GetNewDifficulty();

        // load in left snippet
        LoadPartFromTxtFile(ProjectSettings.GlobalizePath("res://Levels/Parts/Left/LT_10.txt"));

        // select middle snippets
        foreach (string part in rhythm.Split(' '))
        {
            if (part == "_")
            {
                LoadPartFromTxtFile(ProjectSettings.GlobalizePath("res://Levels/Parts/Middle/MX_10.txt"));
            }
            else
            {
                curr_parts_len = snippet_dict["Middle" + diff_str].Length;
                string target_file = $"{snippet_dict["Middle" + diff_str][random.Next(0, curr_parts_len)]}";
                LoadPartFromTxtFile(target_file);

            }
        }

        // load in right snippet
        LoadPartFromTxtFile(ProjectSettings.GlobalizePath("res://Levels/Parts/Right/RT_10.txt"));
        curr_parts_len = snippet_dict["Right" + diff_str].Length;
        LoadPartFromTxtFile($"{snippet_dict["Right" + diff_str][random.Next(0, curr_parts_len)]}");

        // load either connector room or final room
        if (LevelGenVariables.NumRoomsCompleted < LevelGenVariables.NumRooms)
        {
            LoadPartFromTxtFile(ProjectSettings.GlobalizePath(connector_room_path));
        }
        else
        {
            LoadPartFromTxtFile(ProjectSettings.GlobalizePath(final_room_path));
        }

    }

    public void LoadPartFromTxtFile(string part_f, int start_pos_x = -1, int start_pos_y = -1)
    {
        // Load in a part of a level (based on what part we need)
        if (start_pos_x < 0)
        {
            start_pos_x = (int)curr_offset.X;
            start_pos_y = (int)curr_offset.Y;
        }
        Godot.Vector2 start_pos = new(start_pos_x, start_pos_y);

        GetLevelMatFromTxtFile(part_f); // update level_mat to correct output
        BuildLevelFromLevelMat(); // build the level given the current matrix
    }

    public void BuildLevel(ref Level level)
    {
        GD.Print($"Building Level at Position {curr_offset}");

        // loop through <level> to build level
        for (int i = 0; i < level.Layout.Count; i++)
        {
            for (int j = 0; j < level.Layout.Count; j++)
            {
                switch (level.Layout[i][j]) // place respective block in level
                {
                    case "-": break; // empty block -- do nothing
                    case "B":
                        tilemap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { new Vector2I(j + (int)curr_offset.X, i + (int)curr_offset.Y) }, 0, 0);
                        break;
                    case "L":
                        left_connector_pos = new Godot.Vector2(i, j) + curr_offset;
                        break;
                    case "R":
                        right_connector_pos = new Godot.Vector2(i, j) + curr_offset;
                        break;
                    case "!": // case: update position of object
                        this.Position = new((j + (int)curr_offset.X) * BLOCK_SIZE + BLOCK_OFFSET, (i + (int)curr_offset.Y) * BLOCK_SIZE + BLOCK_OFFSET);
                        is_generating_level = false;
                        break;
                    case "K": // case: place key -- check to see if door being generated
                        if (num_keys > 0)
                        {
                            var obj = block_dict["K"].Instantiate();
                            GetTree().Root.CallDeferred("add_child", obj);
                            ((Node2D)obj).Position = new((j + (int)curr_offset.X) * BLOCK_SIZE + BLOCK_OFFSET, (i + (int)curr_offset.Y) * BLOCK_SIZE + BLOCK_OFFSET);
                            num_keys -= 1;
                        }
                        break;
                    default:
                        // case: place object from block dictionary
                        if (block_dict.ContainsKey(level.Layout[i][j]))
                        {
                            var obj = block_dict[level.Layout[i][j]].Instantiate();
                            GetTree().Root.CallDeferred("add_child", obj);
                            ((Node2D)obj).Position = new((j + (int)curr_offset.X) * BLOCK_SIZE + BLOCK_OFFSET, (i + (int)curr_offset.Y) * BLOCK_SIZE + BLOCK_OFFSET);
                        }
                        else { GD.PrintErr($"Error: Block {level.Layout[i][j]} is not in block dict."); }
                        break;
                }
            }
        }
    }

    // Signal Functions
    private void OnBodyEntered(Node other)
    {
        if (other is not Player || other == null) { return; }

        // GD.Print("Player Entered!");
        if (!is_generating_level)
        {
            is_generating_level = true;

            // Update stats for calculating difficulty
            LevelGenVariables.UpdatePlayerStats();

            // Increment level completion trackers
            LevelGenVariables.NumRoomsCompleted += 1;
            LevelGenVariables.NumRoomsGenerated += 1;

            // Update Difficulty
            if (LevelGenVariables.NumRoomsCompleted >= 1 && !LevelGenVariables.PlayerHasSkipped)
            {
                LevelGenVariables.UpdateDifficulty();
            }
            LevelGenVariables.PlayerHasSkipped = false;

            if (num_parts_in_room == 0) { LoadNewRoomFromPartFiles(); }
            else { LoadNewRoomFromPartFiles(num_parts: num_parts_in_room); }
        }
    }

    // ======================
    // == Helper Functions ==
    // ======================

    private void UpdateTilemap(string new_tilemap_path)
    {
        try
        {
            tilemap = GetNode<TileMapLayer>(new_tilemap_path);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error {e}: Could not change tilemap. \"{new_tilemap_path}\" does not exist, or is not a valid tilemap");
        }
    }

    private void UpdateTileset(string new_tileset_path)
    {
        try
        {
            tilemap.TileSet = ResourceLoader.Load<TileSet>(new_tileset_path);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error {e}: Could not cange tileset. \"{new_tileset_path}\" does not exist or is not a valid tileset");
        }
    }

    private static void PlaceObject(string obj_name, Godot.Vector2 pos)
    {
        //Place an object of type <obj_name> at position <pos>
        var obj = block_dict[obj_name].Instantiate();
        ((Node2D)obj).GlobalPosition = pos;
    }

    // DEBUG Functions
    private void PrintRoom()
    {
        string line;
        foreach (var list in level_mat)
        {
            line = "";
            foreach (var obj in list)
            {
                line += obj + " ";
            }
        }
    }

    // Deprecated Functions
    private void BuildLevelFromLevelMat()
    {
        // loop through the level matrix
        for (int i = 0; i < level_mat.Count; i++)
        {
            for (int j = 0; j < level_mat[0].Count; j++)
            {
                if (level_mat[i][j] == "B")
                {
                    tilemap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { new Vector2I(j + (int)curr_offset.X, i + (int)curr_offset.Y) }, 0, 0);
                }
                else
                {
                    Vector2 obj_pos = new((j + (int)curr_offset.X) * BLOCK_SIZE + BLOCK_OFFSET, (i + (int)curr_offset.Y) * BLOCK_SIZE + BLOCK_OFFSET);

                    try { block_dict.ContainsKey(level_mat[i][j]); }
                    catch (Exception e) { GD.PrintErr($"Error {e}: could not find key {level_mat[i][j]}"); }

                    if (block_dict.ContainsKey(level_mat[i][j]))
                    {
                        var obj = block_dict[level_mat[i][j]].Instantiate();
                        // GetTree().Root.AddChild(obj);
                        GetTree().Root.CallDeferred("add_child", obj);
                        ((Node2D)obj).Position = obj_pos;

                        if (level_mat[i][j] == "F")
                        {
                            GD.Print("Flag found.");
                            flag = (Flag)obj;
                        }
                    }
                    else if (level_mat[i][j] == "-") { continue; } // empty element
                    else if (level_mat[i][j] == "L")
                    { // update left connector position
                        left_connector_pos = new Godot.Vector2(i, j) + curr_offset;
                    }
                    else if (level_mat[i][j] == "R")
                    { // update right connector position
                        right_connector_pos = new Godot.Vector2(i, j) + curr_offset;
                    }
                    else if (level_mat[i][j] == "!")
                    { // move block placer here
                        this.Position = obj_pos;
                        is_generating_level = false;
                    }
                    else
                    {
                        GD.PrintErr($"Error: Block {level_mat[i][j]} not in block dictionary.");
                    }
                }
            }
        }

        //update current offset
        if (level_mat.Count == 0) { return; }
        curr_offset += new Godot.Vector2(level_mat[0].Count, 0);
    }

    private List<List<string>> GetLevelMatFromTxtFile(string level_f)
    {
        // Generates a level from a series of <level_f> text files
        level_mat = new(); // clear level matrix

        // check to see if file exists
        if (ResourceLoader.Exists(level_f))
        {
            GD.PrintErr($"file \"{level_f}\" does not exist, or could not be found.");
            return null;
        }

        // load level from file
        int line_number = -1; // current line number in file
        int room_y = 0; // line in the room or part of room

        bool in_room_contents = false; // if true, then currently reading through the room text representation

        using var file = Godot.FileAccess.Open(level_f, Godot.FileAccess.ModeFlags.Read);
        string lines = file.GetAsText();
        Vector2 level_shape = Vector2.Zero;
        foreach (string line in lines.Split('\n'))
        {
            line_number++; // increment line counter

            if (line.Length == 0) { continue; } // empty line so skip

            string[] contents = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (contents[0] == "SHAPE")
            { // initialize array
              // GD.Print($"creating matrix of size ({contents[0]},{contents[1]})");
                level_mat = new List<List<string>>(contents[1].ToInt());
                for (int i = 0; i < contents[1].ToInt(); i++)
                {
                    level_mat.Add(new List<string>(contents[2].ToInt()));
                }
                level_shape = new(contents[1].ToInt(), contents[2].ToInt());
                // GD.Print(level_mat.Count, " ", contents[2].ToInt());
            }
            else if (line == "ROOM")
            {
                in_room_contents = true;
            }
            else if (in_room_contents)
            {
                for (int x = 0; x < contents.Length; x++)
                { // read through line
                    level_mat[room_y].Add(contents[x]);
                }
                room_y++;
            }
        }

        LevelGenVariables.AddRoomToLevelDict(level_f, level_shape);
        return level_mat;
    }
}
