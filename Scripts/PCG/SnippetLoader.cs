using Godot;
using System;
using Mimeva.Utils;

namespace Mimeva.PCG;
public partial class SnippetLoader : Node2D
{
    [Export] private string level_folderpath = "res://Levels/Parts/";
    [Export] private string[] snippet_files = { "res://Levels/Parts/Middle/ME1_10.txt" };

    // Mandatory Objects
    BlockPlacer bp;

    public override void _Ready()
    {

        for (int i = 0; i < snippet_files.Length; i++)
        {
            snippet_files[i] = ProjectSettings.GlobalizePath(snippet_files[i]);
            GD.Print(snippet_files[i]);
        }
        bp = GetNode<BlockPlacer>("/root/World/BlockPlacer");
        bp.ReloadBlockDictionary();
        bp.SetTilemap("/root/World/TileMap/Platforms");
        // bp.SetOffset(new(17, -5));
        bp.LoadPartFromTxtFile(snippet_files[0]);

        // foreach (string snippet_f in snippet_files)
        // {
        //     string snippet_fpath;
        //     if (snippet_f[0] == 'M') snippet_fpath = $"{level_folderpath}Middle/{snippet_f}";
        //     else if (snippet_f[0] == 'L') snippet_fpath = $"{level_folderpath}Left/{snippet_f}";
        //     else snippet_fpath = $"{level_folderpath}Right/{snippet_f}";

        //     snippet_fpath = ProjectSettings.GlobalizePath(snippet_fpath);
        //     Level tmp = new();
        //     tmp.GetLevelFromTxt(snippet_fpath);
        // }
        // Level l = new();
        // l.GetLevelFromTxtParts(snippet_files);
        // GD.Print(l.PrintLevel());
        // bp.BuildLevel(ref l);
    }

    public override void _Process(double delta) { }

}
