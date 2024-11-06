using Godot;
using System;
using System.Numerics;

namespace Mimeva;
public partial class Glorp : Enemy 
{

	[Export] protected Area2D ledge_check = null;
	private bool is_onedge = false;
	public bool IsOnEdge { get => is_onedge; }

	[Export] protected Area2D wall_check; // depracated
 
	public override void _Ready()
	{
		base._Ready();
		sprite.SpeedScale = 1.5f;

		if (ledge_check == null) {
			try { ledge_check = GetNode<Area2D>("LedgeCheck"); }
			catch (Exception e) { GD.PrintErr($"{e}. Ledge Check could not be found."); }
		}

		// ledge_check.AreaEntered += OnLedgeAreaEntered;
		// ledge_check.AreaExited += OnLedgeAreaExited;
		ledge_check.BodyEntered += OnLedgeAreaEntered;
		ledge_check.BodyExited += OnLedgeAreaExited;
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
		if( this.IsOnEdge ) { movespeed = -movespeed; }
		velocity.X = movespeed;

		Velocity = velocity;
	}

	public override void Damage(int damage, bool should_blink=false) {
		base.Damage(damage, should_blink);
	}

	protected void OnLedgeAreaEntered(Node2D other) {
		if(other is TileMapLayer) { is_onedge = false; }
	}

	protected void OnLedgeAreaExited(Node2D other) {
		if(other is TileMapLayer) { is_onedge = true; }
	}
}
