using Godot;

using Mimeva.Entity;
using Mimeva.PCG;

namespace Mimeva.UI;
public partial class PCGUILabel : Label //RichTextLabel
{

    Player player;

    public override void _Ready()
    {
        this.Text = "Difficulty:   \nCompletion:     ";

        /*this.AddThemeFontOverride("", new Godot.Font("res://Fonts/monogram/monogram.ttf"));*/
        /*this.AddThemeFontSizeOverride("", 20);*/

        // Adjust Appearance
        this.Position = new(15, 40);
        this.Size = new(100, 100);

    }

    public override void _Process(double delta)
    {
        this.Text = $"Difficulty: {LevelGenVariables.LevelDifficulty} \nCompletion: {LevelGenVariables.NumRoomsCompleted}/{LevelGenVariables.NumRooms}";
    }
}
