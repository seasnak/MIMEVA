using Godot;
using System;

namespace Mimeva;
public partial class CameraController : Camera2D
{

	CharacterBody2D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try {
			player = GetNode<CharacterBody2D>("/root/World/Player");
		}
		catch {
			GD.Print("Camera could not find Player");
			throw;
		}
		
		this.Zoom = new Vector2(6, 6);
		this.Position = player.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// smooth camera
		Vector2 dist_to_target = player.Position - this.Position;
		if(dist_to_target.Length() > 5) {
			this.Position += dist_to_target.Normalized() * dist_to_target.Length()*0.1f;
		}
	}
	
}
