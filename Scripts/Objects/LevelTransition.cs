using Godot;
using System;

namespace Mimeva;
public partial class LevelTransition : Area2D
{
	// [Export] private PackedScene scene; // the path to the scene to load when the player collides with this level
	private string scene_path;
	
	private AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		
		sprite.Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void OnBodyEntered(Node2D body) {
		
		// if(body is Player) {
		// 	GetTree().ChangeSceneToFile(scene_path);	
		// }
	}

	public void SetLevelPath(string scene_p) { scene_path = scene_p; }
	public string GetLevelPath() { return scene_path; }
}
