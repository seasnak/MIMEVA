using Godot;
using System;
using System.Numerics;

namespace Mimeva;
public partial class Flysp : Enemy 
{
	private Area2D wall_check;

    private Player player; // player node for target

	public override void _Ready()
	{
		base._Ready();

		sprite.SpeedScale = 1.5f;
	}

	public override void _PhysicsProcess(global::System.Double delta)
	{
		base._PhysicsProcess(delta);
		
		// Gravity
		if (Velocity.Y < max_fallspeed) {
			Velocity += new Godot.Vector2( 0, (float)(gravity * 0.5 * delta) );
		}
		else {
			Velocity = new Godot.Vector2( Velocity.X, Math.Min(Velocity.Y, max_fallspeed) );
		}
		SetMovementLogic();
		
		MoveAndSlide();
	}

	public override void _Process(global::System.Double delta)
	{
		base._Process(delta);

		sprite.FlipH = Velocity.X > 0;
		if(Velocity.X != 0) {
			sprite.Play("walk");
		}
		else {
			sprite.Play("idle");
		}
	}

	protected override void SetMovementLogic()
	{
		Godot.Vector2 velocity = Velocity;
		
		if( IsOnWall() ) { movespeed = -movespeed; }
		velocity.X = movespeed;

		Velocity = velocity;
	}

	public override void DealDamage(int damage) {
		base.DealDamage(damage);
	}
}
