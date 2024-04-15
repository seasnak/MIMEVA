using Godot;
using System;

namespace Mimeva;
public partial class Enemy : CharacterBody2D
{
	protected int max_health = 100;
	protected int curr_health = 100;

	protected int max_mana = 100;
	protected int curr_mana = 100;

	protected string enemy_type = "enemy";
	protected int enemy_value = 3;

	protected static readonly float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	protected int movespeed = 20;
	protected int max_fallspeed = (int)(gravity * 10);
	
	protected AnimatedSprite2D sprite;

	// Enemy Conditionals
	private bool is_blinking_state = false;

	// Enemy Timers
	private float damage_blink_timer; // keeps track of player's health
	private float damage_blink_dur = 100; // time (in ms) for how long enemy sprite blinks

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
		// check death
		if(curr_health <= 0) {
			HandleDeath();
		}

		// handle damage blink
		if(is_blinking_state && Time.GetTicksMsec() - damage_blink_timer >= damage_blink_dur) {
			this.Modulate = new Godot.Color(1, 1, 1);
			is_blinking_state = false;
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
		this.Modulate = new Godot.Color(0, 0, 0);
		damage_blink_timer = Time.GetTicksMsec();
		is_blinking_state = true;
		this.curr_health -= damage;
	}

}
