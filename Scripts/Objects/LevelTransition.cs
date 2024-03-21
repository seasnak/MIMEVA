using Godot;
using System;

namespace Mimeva;
public partial class LevelTransition : Area2D
{
	[Export] private PackedScene scene; // the path to the scene to load when the player collides with this level
	[Export] private string scene_path;
	
	private AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("default");
		
		scene_path ??= scene.ResourcePath; // if scene path is null then set it to the path of the scene PackedScene
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void OnBodyEntered(Node2D body) {
		
		if(body is Player) {
			GD.Print("Player Entered Level Transition");
			GetTree().ChangeSceneToFile(scene_path);
		}
	}

	public void SetLevelPath(string scene_p) { scene_path = scene_p; }
	public string GetLevelPath() { return scene_path; }
}
