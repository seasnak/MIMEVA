using Godot;
using System;
// using System.Numerics;

namespace Mimeva;
public partial class Glorp : Enemy 
{

	[Export] protected Area2D ledge_check = null;
	private bool is_onedge = false;
	public bool IsOnEdge { get => is_onedge; }

	// knockback variables
	private bool is_knockedback = false;
	public bool IsKnockedBack{ get => is_knockedback; }
	protected float knockback_dist = 3;
	protected uint knockback_dur_msec = 500;
	private ulong knockback_timer = 0;

	// External Nodes
	protected Player player;
 
	public override void _Ready()
	{
		base._Ready();
		sprite.SpeedScale = 1.5f;

		if (ledge_check == null) {
			try { ledge_check = GetNode<Area2D>("LedgeCheck"); }
			catch (Exception e) { 
				GD.PrintErr($"{e}. Ledge Check could not be found."); 
				throw; 
			}
		}
		
		try { player = GetParent().GetNode<Player>("Player"); }
		catch (Exception e) {
			GD.PrintErr($"{e}. Player could not be found");
			throw;
		}

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
		ledge_check.GetNode<CollisionShape2D>("CollisionShape2D").Position = new(4 * Velocity.X > 0 ? 1 : -1, 7);
		if(Velocity.X != 0) {
			sprite.Play("walk");
		}
		else {
			sprite.Play("idle");
		}

		if(is_knockedback && Time.GetTicksMsec() >= knockback_timer + knockback_dur_msec) { is_knockedback = false; }
	}

	protected override void SetMovementLogic() {
		Godot.Vector2 velocity = Velocity;

		if( IsOnWall() ) { movespeed = -movespeed; }
		if( this.IsOnEdge ) { movespeed = -movespeed; }
		velocity.X = movespeed;
		
		Velocity = velocity;
	}

	public override void Damage(int damage, bool should_blink=false) {
		base.Damage(damage, should_blink);
		
		// knockback handler
		if(!is_knockedback) {
			if(player.WeaponRotationDeg == 0) { this.Position -= new Godot.Vector2(-5, 0); }
			else if(player.WeaponRotationDeg == 180) { this.Position -= new Godot.Vector2(5, 0); GD.Print("Glorp Knocked Back");}
			else { return; } // player attacked from above or below
			is_knockedback = true;
			knockback_timer = Time.GetTicksMsec();
		}

		if(player.WeaponRotationDeg == 0) { this.Position -= new Godot.Vector2(-5, 0); }
		else if(player.WeaponRotationDeg == 180) { this.Position -= new Godot.Vector2(5, 0); GD.Print("Glorp Knocked Back");}
		else { return; } // player attacked from above or below
	}

	protected void OnLedgeAreaEntered(Node2D other) {
		if(other is TileMapLayer) { is_onedge = false; }
	}

	protected void OnLedgeAreaExited(Node2D other) {
		if(other is TileMapLayer) { is_onedge = true; }
	}
}
