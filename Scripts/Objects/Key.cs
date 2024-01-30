using Godot;
using System;

namespace Mimeva;
public partial class Key : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		BodyEntered += OnAreaEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAreaEntered(Node body) {
		if(body == null || body is not Player) { return; }
		
		((Player)body).AddKeys(1);
		QueueFree();
	}
}
