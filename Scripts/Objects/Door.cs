using Godot;
using System;

namespace Mimeva;
public partial class Door : Area2D {

  private bool is_active = true; // if the door is open, then it is set to false
	private Key required_key;
  private StaticBody2D collider;
  private AnimatedSprite2D sprite;

  private Player player; // the target player for scope

  public override void _Ready()
  {
	collider = GetNode<StaticBody2D>("StaticBody2D");
	// this.CollisionLayer = 1;
	// this.CollisionMask = 1;

	sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	BodyEntered += OnBodyEntered;

	sprite.Play("closed");
  }

  public override void _Process(double delta)
  {

  }

	private void OnBodyEntered(Node body) {
  
	if(body is Player) {
	  int keys = ((Player)body).GetKeys();
	  if(keys > 0) {
		this.is_active = false;
		collider.QueueFree();
		sprite.Play("open");
		((Player)body).SetKeys(keys - 1);
	  }
	}

	}



}
