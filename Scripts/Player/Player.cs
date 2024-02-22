using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Mimeva;
public partial class Player : CharacterBody2D
{
	
	// Player Stats
	private int max_health = 100;
	private int curr_health = 100;
	private int max_stamina = 100;
	private int curr_stamina = 100;

	private int death_count = 0;
	

	private int coins = 0;
	private int keys = 0;
	private int cool_coins = 0; // the harder to get bonus coins

	// Movement Vals	
	private const int movespeed = 70;
	private const int low_jumpspeed = 65; // 4.5
	private const int high_jumpspeed = 150; // 10
	private const int fast_fallspeed = 30;
	private const int dashspeed = 100;
	private const int max_fallspeed = 200;
	private const int attack_dur = 240; // duration of the attack in milliseconds
	private ulong start_time = 0;

	// Player Boolean Checks
	private bool is_grounded = true;
	private bool is_held_jump = false;
	private bool has_fastfell = false;
	private bool can_climb = false;
	private bool is_climbing = false;
	private bool is_attacking = false;

	private bool is_dead = false;

	// Related Nodes
	private AnimatedSprite2D sprite;
	private CollisionShape2D collider;
	private PlayerVariables player_vars;
	private HitBox weapon;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {	
		sprite = (AnimatedSprite2D)(GetNode("AnimatedSprite2D"));
		// player_vars = (PlayerVariables)GetNode("/root/PlayerVariables"); // TODO: add player variables
		weapon = (HitBox)GetNode("Sword");
		weapon.SetDamage(30);

		collider = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(double delta)
	{

		if(is_dead) {return;} // prevent physics if the player is dead

		// Gravity
		if (IsOnFloor()) {
			// player is on floor so don't apply gravity
		}
		else if (Velocity.Y < max_fallspeed) {
			Velocity += new Godot.Vector2( 0, (float)(gravity * 0.5 * delta) );
		}
		else {
			Velocity = new Godot.Vector2( Velocity.X, Math.Min(Velocity.Y, max_fallspeed) );
		}
		
		Godot.Vector2 input_dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		HandleMove(input_dir);
		HandleJump(input_dir);
		HandleAttack();
		
		MoveAndSlide();
	}

    public override void _Process(double delta)
    {

		// Handle Health and Death
		if (curr_health <= 0) {

			if(!is_dead) {
				GD.Print("Player Died!");
				
				sprite.Play("death");
				is_dead = true;
				death_count += 1;
			}
			else if(!sprite.IsPlaying()) {
				Die();
				is_dead = false;
			}
		}
		
    }

    private void Die() {
		try{ this.Position = player_vars.checkpoint.Position; }
		catch{ this.Position = Godot.Vector2.Zero; }

		this.Velocity = Godot.Vector2.Zero;
		curr_health = max_health;
	}

	private void HandleAttack() {
		
		if(Input.IsActionJustPressed("attack") && !is_attacking) {
			// GD.Print("Player attacking"); // DEBUG
			is_attacking = true;
			// sprite.Play("attack");
			weapon.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
			weapon.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("slash");
			start_time = Time.GetTicksMsec();
		}

		if(is_attacking && Time.GetTicksMsec() - start_time >= attack_dur ) {
			is_attacking = false;
			weapon.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		}
	}

	private void HandleMove(Godot.Vector2 input_dir) {
		Velocity = new Godot.Vector2(movespeed * input_dir.X, Velocity.Y);
		
		if(input_dir.X != 0 && !is_attacking) {
			sprite.FlipH = input_dir.X > 0;
			collider.Position = new Godot.Vector2(-0.5 * input_dir.X > 0 ? -1 : 1, collider.Position.Y);

			weapon.GetSprite().FlipH = sprite.FlipH;
			weapon.Position = new Godot.Vector2(8 * input_dir.X, 0);
			
		}
		if(input_dir.X != 0 && is_grounded) {
			sprite.Play("walk");
		}
		else if(can_climb && input_dir.Y != 0) {
			is_climbing = true;
			sprite.Play("idle");
		}
		
		else if(!is_grounded) {
			// TODO: add jumping animation
			sprite.Play("jump");
		}
		else {
			sprite.Play("idle");
		}
	}

	private void HandleJump(Godot.Vector2 input_dir) {
		Godot.Vector2 velocity = Velocity; // tmp variable to make calculations easier 
		is_grounded = this.IsOnFloor();

		int dir = input_dir.X > 0 ? 1 : -1;
		if(WallJumpCheck(dir)) {
			velocity = WallJump(dir, velocity);
		}

		if (is_grounded) {
			has_fastfell = false;
			if (Input.IsActionJustPressed("jump")) {
				velocity.Y = -high_jumpspeed;
				is_held_jump = true;
			}
		}
		else {
			// variable jump height
			if (Input.IsActionJustReleased("jump") && velocity.Y < -low_jumpspeed) {
				velocity.Y = -low_jumpspeed;
				is_held_jump = false;
			}

			// steeper jump downward arc for better jump game feel
			if (!has_fastfell && velocity.Y > 0) {
				// this.Velocity += new Vector2(0, fast_fallspeed);
				velocity.Y = fast_fallspeed;
				has_fastfell = true;
			}
		}
		
		Velocity = velocity;
	}

	private bool WallJumpCheck(int dir) { // TODO: fix to make it more fluid
		return IsOnWall();
	}

	private Godot.Vector2 WallJump(int dir, Godot.Vector2 velocity) { // wall jump in the direction dir
		
		has_fastfell = false;
		if (Input.IsActionJustPressed("jump")) {
			velocity.Y = -high_jumpspeed * 0.75f;
			velocity.X = -200 * dir; // wall bounceback
			is_held_jump = true;
		}
		
		return velocity;
	}

	public void AddCurrency(int val) { this.coins += val; }
	public void DealDamage(int val) { this.curr_health -= val; }


	//Getters and Setters 
	public void SetClimb(bool val) { this.can_climb = val; }
	public bool GetClimb() { return this.can_climb; }

	public int GetKeys() { return keys; }
	public void SetKeys(int val) { keys = val; }
	public void AddKeys(int val) { keys += val; }

	public int GetCurrHealth() { return this.curr_health; }
	public void SetCurrHealth(int val) { this.curr_health = val; }
	public int GetMaxHealth() { return this.max_health; }
	public void SetMaxHealth(int val) { this.max_health = val; }

	public int GetCoins() { return this.coins; }
	
	public bool IsAttacking() { return is_attacking; }
	
}
