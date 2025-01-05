using System;
using Godot;

using Mimeva;
namespace Mimeva.PCG;

public partial class LevelGen : Node2D
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

	public void ModifyPart()
	{

	}


}
