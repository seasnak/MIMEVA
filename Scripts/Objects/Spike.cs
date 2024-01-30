using Godot;
using System;

namespace Mimeva;
public partial class Spike : Area2D
{

	private const int damage = 99;
	private bool is_solid = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.BodyEntered += OnBodyEntered; // Connect BodyEntered Signal
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player) {
			((Player)body).DealDamage(damage);
		}
		else if (body is Enemy) {
			((Enemy)body).DealDamage(damage);
		}
	}
}
