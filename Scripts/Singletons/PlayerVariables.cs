using Godot;
using System;
using System.Collections.Generic;

using Mimeva.Object;

namespace Mimeva.Entity;
public partial class PlayerVariables : Node
{

    private static Checkpoint curr_checkpoint = null;
    private static string checkpoint_scene_path = null;
    private static Node checkpoint_scene = null;

    private static Godot.Vector2 player_starting_pos;
    private static int num_deaths = 0; // the number of player deaths
    /*public static int NumDeaths { get => num_deaths; }*/

    private static int enemy_killcount = 0; // the number of enemies that the player has killed
    public static int EnemyKillCount { get => enemy_killcount; }

    private static List<string> enemy_killlist = new(); // the list of all of his enemies
    private static int num_coins = 0;
    public static int NumCoins { set => num_coins = value; get => num_coins; }

    /*private static Vector2[] player_death_pos = {};/*/
    private static Godot.Collections.Array player_death_pos_arr = new();
    /*private static Godot.Collections.Dictionary player_deaths_dict = new(); // dict containing the name of the object that killed the player and the number of times said player has died to that object.*/
    private static System.Collections.Generic.Dictionary<string, int> player_deaths_dict = new();
    public System.Collections.Generic.Dictionary<string, int> PlayerDeathsDict { get => player_deaths_dict; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // To do move to a different file at some point
        if (Input.IsKeyPressed(Godot.Key.Escape))
        {
            GetTree().Quit();
        }
    }

    public static void AddPlayerDeath(Godot.Vector2 pos, String cause_of_death, int curr_diff)
    {
        num_deaths += 1;
        player_death_pos_arr.Add(pos);

        if (player_deaths_dict.ContainsKey(cause_of_death))
        {
            player_deaths_dict[cause_of_death] += 1;
        }
        else
        {
            player_deaths_dict[cause_of_death] = 1;
        }

    }

    public static void AddEnemyKill(string enemy_name)
    {
        enemy_killcount += 1;
        enemy_killlist.Add(enemy_name);
    }

    // Getters and Setters
    public static int NumDeaths { get => num_deaths; set => num_deaths = value; }

    // Checkpoint Getter Setters
    public static Checkpoint GetCheckpoint()
    {
        return curr_checkpoint;
    }
    public static void SetCheckpoint(Checkpoint new_checkpoint)
    {
        curr_checkpoint = new_checkpoint;
    }
    public static Godot.Vector2 GetCheckpointPos()
    {
        return curr_checkpoint.GlobalPosition;
    }
    public static Node GetCheckpointScene()
    {
        return checkpoint_scene;
    }
    public static void SetChekpointScene(Node scene)
    {
        checkpoint_scene = scene;
    }
    public static string GetCheckpointScenePath()
    {
        return checkpoint_scene_path;
    }
    public static void SetCheckpointScenePath(string scene_path)
    {
        checkpoint_scene_path = scene_path;
    }

    // Player Getter/Setters
    public static Godot.Vector2 GetPlayerStartingPos()
    {
        return player_starting_pos;
    }
    public static void SetPlayerStartingPos(Godot.Vector2 pos)
    {
        player_starting_pos = pos;
    }

}
