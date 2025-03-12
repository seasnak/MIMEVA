using Godot;

using Mimeva.PCG;

namespace Mimeva.UI;
public partial class MainMenuList : Label
{
    private string[] items = new string[] { $"Starting Difficulty ", $"Number of Rooms: " };
    private int cursor = 0;

    public override void _Ready()
    {
        this.HorizontalAlignment = HorizontalAlignment.Center;
        this.Position = new(276, 400);

        RedrawMainMenuList();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Down"))
        {
            cursor = Mathf.Min(items.Length - 1, cursor + 1);
        }
        else if (Input.IsActionJustPressed("Up"))
        {
            cursor = Mathf.Max(0, cursor - 1);
        }

        if (Input.IsActionJustPressed("Right"))
        {
            UpdateLevelGenVariable(1);
        }
        else if (Input.IsActionJustPressed("Left"))
        {
            UpdateLevelGenVariable(-1);
        }

        RedrawMainMenuList();
    }

    private string RedrawMainMenuList()
    {
        string label = "";
        for (int i = 0; i < items.Length; i++)
        {
            if (cursor == i)
            {
                label += "=> ";
            }

            label += items[i] + " ";
            switch (i)
            {
                case 0:
                    label += LevelGenVariables.LevelDifficulty;
                    break;
                case 1:
                    label += LevelGenVariables.NumRooms;
                    break;
                default:
                    GD.Print($"Error getting global value for {items[i]}");
                    break;
            }
            label += "\n";
        }

        this.Text = label;
        return label;
    }

    private void UpdateLevelGenVariable(int change)
    {
        switch (cursor)
        {
            case 0:
                float diff = LevelGenVariables.LevelDifficulty + change;
                diff = Mathf.Min(10, Mathf.Max(0, diff));
                LevelGenVariables.LevelDifficulty = diff;
                break;
            case 1:
                int numr = LevelGenVariables.NumRooms + change;
                numr = Mathf.Min(10, Mathf.Max(1, numr));
                LevelGenVariables.NumRooms = numr;
                break;
            default:
                GD.Print($"Couldn't find case {cursor}");
                break;
        }
    }
}
