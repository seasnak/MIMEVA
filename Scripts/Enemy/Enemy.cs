using Godot;
using System;

namespace Mimeva;
public partial class Enemy : CharacterBody2D
{
	protected int max_health = 10;
	protected int curr_health = 10;

	protected int max_mana = 10;
	protected int curr_mana = 10;

	protected string enemy_type = "enemy";
	protected int enemy_value = 3;

	protected static readonly float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	protected int movespeed = 10;
	protected int max_fallspeed = (int)(gravity * 10);

	protected AnimatedSprite2D sprite;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AddToGroup("enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
	}

    public override void _Process(double delta)
    {
		if(curr_health <= 0) {
			HandleDeath();
		}

    }
	
	private void HandleDeath() {
		//TODO: Add death animation player
		QueueFree();
	}

	protected virtual void SetMovementLogic() 
	{
		
	}

	public virtual void DealDamage(int damage) { 
		
		GD.Print($"Dealing {damage} damage to enemy");
		this.curr_health -= damage;

		
	}

}
