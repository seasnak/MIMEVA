using Godot;

using Mimeva.PCG;
using Mimeva.Entity;

namespace Mimeva.UI;
public partial class StatsController : Control
{

    [Export] private Label stats_label;

    public override void _Ready()
    {
        stats_label = GetNode<Label>("Stats");
        stats_label.Text = GenerateStatsLabel();
    }

    public override void _Process(double delta)
    {

        if (Input.IsActionPressed("jump"))
        {
            GetTree().ChangeSceneToFile("res://Scenes/Menus/mainmenu.tscn");
        }

    }

    private string GenerateStatsLabel()
    {
        int score = CalculateScore(PlayerVariables.NumDeaths, PlayerVariables.NumCoins, PlayerVariables.EnemyKillCount, LevelGenVariables.LevelDifficulty);
        return $"===   POINTS   ===\nDeaths:     {PlayerVariables.NumDeaths}\nCoins:     {PlayerVariables.NumCoins}\nEnemies     Killed:      {PlayerVariables.EnemyKillCount}\nFinal     Difficulty:     {LevelGenVariables.LevelDifficulty}\n--------------------\nTotal    Score:     {score}";
    }

    private int CalculateScore(int deaths, int coins, int enemies_killed, float difficulty)
    {
        return (int)(difficulty * 10) + (coins * 2) + (enemies_killed * 5) - (deaths * 5);
    }

}
