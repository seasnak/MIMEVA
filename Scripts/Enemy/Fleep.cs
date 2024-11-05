using Godot;
using System;
using System.Numerics;
using Mimeva.Projectiles;

namespace Mimeva;
public partial class Fleep : Enemy 
{

	[Export] protected PackedScene bullet; 

	[Export] protected int keepaway_dist = 30; // the minimum distance fleep keeps away from the player
	[Export] protected int alert_range = 60; // the minimum distance at which fleep is alerted to the player
    [Export] protected Player player = null; // player node for target

	[Export] protected int movespeed_override = -1;

	// movement vars
	private bool is_backingup = false;
	[Export] protected int backup_dur_msec = -1;
	private double backup_starttime = 0;
	private bool is_passmove = false;
	[Export] protected int passmove_dur_msec = -1;
	private double passmove_starttime = 0;

	private bool has_attacked = true;
	[Export] protected int attack_cooldown_msec = 3000;
	private double attackcd_starttime = 0;

	public override void _Ready()
	{
		base._Ready();

		// update stats
		this.movespeed = movespeed_override < 0 ? 20 : movespeed_override;
		this.max_health = 50;

		// update sprite
		sprite.SpeedScale = 1.5f;
		sprite.Play("default");

		// find nodes
		player = GetNode<Player>("/root/World/Player");
	}

	public override void _PhysicsProcess(global::System.Double delta)
	{
		base._PhysicsProcess(delta);
		SetMovementLogic();
		MoveAndSlide();
	}

	public override void _Process(global::System.Double delta)
	{
		base._Process(delta);
		
		// GD.Print($"backup: {is_backingup}, passmove: {is_passmove}, attacked: {has_attacked}"); // DEBUG

		Godot.Vector2 vec_to_player = this.GlobalPosition - player.GlobalPosition;
		sprite.FlipH = is_backingup ? vec_to_player.Normalized().X < 0 : Velocity.X > 0;
		
		// manage movement and attack booleans
		if(backup_starttime + backup_dur_msec <= Time.GetTicksMsec()) { is_backingup = false; }
		if(passmove_starttime + passmove_dur_msec <= Time.GetTicksMsec()) { is_passmove = false; }
		if(attackcd_starttime + attack_cooldown_msec <= Time.GetTicksMsec()) { has_attacked = false; }
	}

	protected override void SetMovementLogic()
	{
		Godot.Vector2 velocity = Velocity;

		if(is_passmove) { return; }
		
		Godot.Vector2 vec_to_player = this.GlobalPosition - player.GlobalPosition;
		Random random = new();
		if(vec_to_player.Length() > alert_range) { // passive movement
			is_passmove = true;
			passmove_dur_msec = passmove_dur_msec < 0 ? random.Next(0, 200) + 1000 : passmove_dur_msec;
			passmove_starttime = Time.GetTicksMsec();

			velocity = random.Next(100) > 50 ? Godot.Vector2.Zero : new(movespeed * random.Next(-10, 10)/10, movespeed * random.Next(-10, 10)/20);
		}
		else if(vec_to_player.Length() > keepaway_dist && !is_backingup) { // regular movement
			velocity = -vec_to_player.Normalized() * movespeed;
		}
		else{ // backing up
			is_backingup = true;
			backup_dur_msec = backup_dur_msec < 0 ? random.Next(0, 500) + 500 : backup_dur_msec;
			backup_starttime = Time.GetTicksMsec();
			velocity = vec_to_player.Normalized() * movespeed;
		}

		if(!has_attacked && !is_passmove) {
			int attack_chance = random.Next(100);
			// GD.Print(attack_chance); // DEBUG

			if(attack_chance > 50) {
				has_attacked = true;
				attackcd_starttime = Time.GetTicksMsec();
				
				// instantiate a bullet
				Bullet b = bullet.Instantiate() as Bullet;
				// this.CallDeferred("add_sibling", b);
				this.AddSibling(b);
				b.Position = this.Position;
				b.Target = player;
				b.ResetTargetAngle();
			}
		}
		Velocity = velocity;
	}

	public override void Damage(int damage, bool should_blink=false) {
		base.Damage(damage, should_blink);
	}
}
