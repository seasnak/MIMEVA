using Godot;
using System;

namespace Mimeva;
public partial class PlayerVariables : Node
{


	public Respawner respawner = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private Respawner GetRespawn() {
		return respawner;
	}

	private Vector2 GetRespawnLocation() {
		return respawner.Position;
	}
}
