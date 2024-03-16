using Godot;
using System;

namespace Mimeva;
public partial class PlayerVariables : Node
{


	private static Checkpoint checkpoint = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}


	// Getters and Setters
	public static Checkpoint GetCheckpoint() {
		return checkpoint;
	}

	public static Vector2 GetCheckpointPos() {
		return checkpoint.Position;
	}

	public static void SetCheckpoint(Checkpoint new_checkpoint) {
		checkpoint = new_checkpoint;
	}
}
