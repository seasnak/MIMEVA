using Godot;
using System;

namespace Mimeva;
public partial class HealthBar : TextureProgressBar
{
	
	[Export] private Player player;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("/root/World/Player");
		
		// this.ShowPercentage = false;
		this.Position = new Vector2(10, 10);
		this.Size = new Vector2(5 * player.GetMaxHealth(), 20); // scale bar with player stats 
		this.Scale = new Vector2(3, 3);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// update bar
		this.Value = player.GetCurrHealth() * this.MaxValue / player.GetMaxHealth();
	}
}
