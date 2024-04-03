using Godot;
using System;

namespace Mimeva;
public partial class LevelTransition : Area2D
{
	[Export] private PackedScene scene; // the path to the scene to load when the player collides with this level
	[Export] private string scene_path;
	
	private AnimatedSprite2D sprite;
	
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("default");

		player = GetNode<Player>("/root/World/Player");
		
		scene_path ??= scene.ResourcePath; // if scene path is null then set it to the path of the scene PackedScene
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		// hide sprite if player isn't close
		float a = Math.Max(0, 50 - (this.Position - player.Position).Length())/30;
		this.Modulate = new Godot.Color(Modulate.R, Modulate.G, Modulate.B, a);
		
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
