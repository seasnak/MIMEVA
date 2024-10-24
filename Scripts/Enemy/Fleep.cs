using Godot;
using System;
using System.Numerics;

namespace Mimeva;
public partial class Fleep : Enemy 
{

	[Export] private Area2D projectile; 

	[Export] private int keepaway_dist = 30; // the minimum distance fleep keeps away from the player
	[Export] private int alert_range = 60; // the minimum distance at which fleep is alerted to the player
    [Export] private Player player = null; // player node for target

	[Export] private int movespeed_override = -1;

	// movement vars
	private bool is_backingup = false;
	private int backup_dur_msec = -1;
	private double backup_starttime = 0;
	private bool is_passmove = false;
	private int passmove_dur_msec = -1;
	private double passmove_starttime = 0;

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

		sprite.FlipH = Velocity.X > 0;

		if(backup_starttime + backup_dur_msec <= Time.GetTicksMsec()) { is_backingup = false; }
		if(passmove_starttime + passmove_dur_msec <= Time.GetTicksMsec()) { is_passmove = false; }
	}

	protected override void SetMovementLogic()
	{
		Godot.Vector2 velocity = Velocity;
		
		Godot.Vector2 vec_to_player = (this.GlobalPosition - player.GlobalPosition);
		Random random = new();
		if(vec_to_player.Length() > alert_range && !is_passmove) { // passive movement
			is_passmove = true;
			passmove_dur_msec = passmove_dur_msec < 0 ? random.Next(0, 200) + 1000 : passmove_dur_msec;
			passmove_starttime = Time.GetTicksMsec();
			velocity = new(movespeed * random.Next(-10, 10)/10, movespeed * random.Next(-10, 10)/20);
		}
		else if(vec_to_player.Length() > keepaway_dist && !is_backingup) { // regular movement
			velocity = -vec_to_player.Normalized() * movespeed;
		}
		else if(!is_passmove){ // not passive moving
			is_backingup = true;
			backup_dur_msec = backup_dur_msec < 0 ? random.Next(0, 200) + 1000 : backup_dur_msec;
			backup_starttime = Time.GetTicksMsec();
			velocity = vec_to_player.Normalized() * movespeed;
		}

		GD.Print();
		Velocity = velocity;
	}

	public override void Damage(int damage) {
		base.Damage(damage);
	}
}
