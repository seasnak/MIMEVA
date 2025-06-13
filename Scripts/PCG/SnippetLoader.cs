using Godot;
using System;
using Mimeva.Utils;

namespace Mimeva.PCG;
public partial class SnippetLoader : Node2D
{
    // [Export] private string level_folderpath = "res://Levels/Parts/";
    [Export] private string[] snippet_files = { "res://Levels/Parts/Middle/ME1_10.txt" };

    // Mandatory Objects
    BlockPlacer bp;

    public override void _Ready()
    {
        bp = GetNode<BlockPlacer>("/root/World/BlockPlacer");
        bp.ReloadBlockDictionary();
        bp.SetTilemap("/root/World/TileMap/Platforms");
        // bp.SetOffset(new(17, -5));
        for (int i = 0; i < snippet_files.Length; i++)
        {
            snippet_files[i] = ProjectSettings.GlobalizePath(snippet_files[i]);
            GD.Print(snippet_files[i]);
            bp.LoadPartFromTxtFile(snippet_files[i]);
        }
    }

    public override void _Process(double delta) { }

}
