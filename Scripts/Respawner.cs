using Godot;
using System;

namespace Mimeva;
public partial class Respawner : Area2D
{

	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) 
	{
		
	}
	
	public void SpawnPlayer(Player target) 
	{
        target.Position = this.Position;
    }

    public void SpawnPlayer() 
	{
        player.Position = this.Position;
    }
}
