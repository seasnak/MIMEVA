using Godot;
using System;
using System.Collections;

namespace Mimeva;
public partial class Coin : Area2D
{

	public override void _Ready() 
	{
		BodyEntered += OnBodyEntered;
	}

    public override void _Process(double delta)
    {
        
    }

	private void OnBodyEntered(Node body) {
		if(body == null || body is not Player) { return; }

		((Player)body).AddCurrency(1);
		QueueFree();
	}

}
