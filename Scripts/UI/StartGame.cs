using Godot;
using System;

namespace Mimeva;
public partial class StartGame : Label
{
	[Export] private string starting_scene_path = "";
	private float curr_alpha = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		curr_alpha += 2*(float)delta;
		this.Modulate = new Godot.Color(this.Modulate.R, this.Modulate.G, this.Modulate.B, (1+(float)Math.Sin(curr_alpha))/2);

		if(Input.IsKeyPressed(Godot.Key.Z)) {
			try { 
				GetTree().ChangeSceneToFile(starting_scene_path);
			}
			catch { 
				GD.Print("error loading scene!"); 
				GetTree().Quit(); 
			}
		}
	}
}
