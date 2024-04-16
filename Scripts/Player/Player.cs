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

	// Movement Values	
	private const int movespeed = 60;
	private const int low_jumpspeed = 75; // 4.5
	private const int high_jumpspeed = 150; // 10
	private const int walljump_knockback = 80;
	private const int fast_fallspeed = 30;
	private const int dashspeed = 160;
	private const int air_dashspeed = 120;
	private const int max_fallspeed = 180;

	// Player Timer Values
	private const int attack_dur = 240; // duration of the attack in milliseconds
	private const int dash_dur = 130; // dash duration in msec
	private const int dash_lockout_time = 200; // lockout on the dash
	private const int walljump_dur = 60; // the time for the player to be knocked back
	private ulong curr_attack_time = 0; // current atk time for timer purposes
	private ulong curr_dash_time = 0; // current dash time for timer purposes
	private ulong curr_walljump_time = 0; 

	// Player Boolean Checks
	private bool is_grounded = true;
	private bool is_held_jump = false;
	private bool has_fastfell = false;
	private bool can_climb = false;
	private bool is_climbing = false;
	private bool is_attacking = false;
	private bool is_dashing = false;
	private bool dash_is_airdash = false; // determines whether the dash was an airdash
	private bool is_jumping = false;
	private bool is_walljumping = false;

	private bool can_dash = true; // check for player can only dash once while in the air
	private bool can_jump = true; // check to see if player can jump
	
	private bool is_dead = false;
	
	// Related Nodes
	private AnimatedSprite2D sprite;
	private CollisionShape2D collider;
	private PlayerVariables p_vars;
	private HitBox weapon;

	// Debug Nodes
	private Godot.Collections.Dictionary<string,Line2D> debugline_dict;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {	
		sprite = (AnimatedSprite2D)(GetNode("AnimatedSprite2D"));
		p_vars = (PlayerVariables)GetNode("/root/PlayerVariables");
		weapon = (HitBox)GetNode("Sword");
		weapon.SetDamage(30);

		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		debugline_dict = new Godot.Collections.Dictionary<string,Line2D>();

		PlayerVariables.SetPlayerStartingPos(this.Position);
	}

	public override void _PhysicsProcess(double delta)
	{

		if(is_dead) {return;} // prevent physics if the player is dead
		
		// Gravity
		if (IsOnFloor()) {
			// player is on floor so don't apply gravity
			is_jumping = false;
			is_walljumping = false;
		}
		else if (is_dashing) {
			Velocity = new(Velocity.X, 0);
		}
		else if (Velocity.Y < max_fallspeed) {
			Velocity += new Godot.Vector2( 0, (float)(gravity * 0.5 * delta) );
		}
		else {
			Velocity = new( Velocity.X, Math.Min(Velocity.Y, max_fallspeed) );
		}
		
		// Update player variables
		is_grounded = IsOnFloor();
		
		Godot.Vector2 input_dir = new(Input.GetAxis("ui_left", "ui_right"), Input.GetAxis("ui_up", "ui_down"));
		HandleMove(input_dir, (float) delta);
		HandleJump(input_dir);
		HandleAttack();
		
		MoveAndSlide();
	}

    public override void _Process(double delta)
    {
		// Handle Health and Death
		if (curr_health <= 0) {
			
			if(!is_dead) {
				// GD.Print("Player Died!"); // DEBUG
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
		try{
			// if() {
			// 	GD.Print("loading scene");
			// 	GetTree().ChangeSceneToFile(PlayerVariables.GetCheckpointScenePath());
			// }
			this.GlobalPosition = PlayerVariables.GetCheckpointPos(); // set player position to the position of the checkpoint
		}
		catch{ 
			this.Position = PlayerVariables.GetPlayerStartingPos(); 
		}
		
		this.Velocity = Godot.Vector2.Zero;
		curr_health = max_health;
	}

	private void HandleAttack() {
		
		if(is_dashing) { return; } // prevent player from attacking while dashing

		if(Input.IsActionJustPressed("attack") && !is_attacking) {
			is_attacking = true;
			// sprite.Play("attack");
			weapon.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
			weapon.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("slash");
			curr_attack_time = Time.GetTicksMsec();
		}

		if(is_attacking && Time.GetTicksMsec() - curr_attack_time >= attack_dur ) {
			is_attacking = false;
			weapon.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		}
		
	}

	private void HandleMove(Godot.Vector2 input_dir, float delta = 1) {

		// handle player timers
		if(is_dashing && Time.GetTicksMsec()-curr_dash_time >= dash_dur) { 
			is_dashing = false;
		}
		else if(is_walljumping && Time.GetTicksMsec()-curr_walljump_time >= walljump_dur) {
			is_walljumping = false;
		}

		// handle player movement
		Godot.Vector2 velocity = Velocity;
		
		if(is_dashing) {
			// GD.Print("is_dashing");
		}
		else if(is_walljumping) {
			// GD.Print("is_walljumping");
		}
		if(velocity.X != movespeed * input_dir.X) { // not dashing or walljumping so keep momentum
			// slow down movement only if player is on the ground
			if(is_grounded) {
				if(input_dir.X > 0 && velocity.X > input_dir.X * movespeed ) {
					velocity.X -= 500 * delta;
				}
				else if(input_dir.X < 0 && velocity.X < input_dir.X * movespeed) {
					velocity.X += 500 * delta;
				}
				else{
					velocity.X = input_dir.X * movespeed;
				}
			}
			else { // player is either jumping or is airborne
				if(Math.Sign(velocity.X) != input_dir.X) {
					velocity.X = input_dir.X * movespeed;
				}
			}
		}

		// handle character sprite and weapon
		if(input_dir.X != 0 && !is_attacking && !is_dashing) {
			sprite.FlipH = input_dir.X > 0;
			collider.Position = new Godot.Vector2(-0.5 * input_dir.X > 0 ? -1 : 1, collider.Position.Y);

			weapon.GetSprite().FlipH = sprite.FlipH;
			weapon.Position = new Godot.Vector2(8 * input_dir.X, 0);
		}

		// handle dash movement
		if(!is_dashing) {
			if(can_dash && input_dir.X != 0 && Input.IsActionJustPressed("dash")) {
				is_dashing = true;
				can_dash = false;
				velocity = HorDash(Math.Sign(input_dir.X));
			}
			
			if(!can_dash && is_grounded && Time.GetTicksMsec()-curr_dash_time>dash_lockout_time) { // reset dash if player is on ground and not locked out
				curr_dash_time = Time.GetTicksMsec();
				// GD.Print("dash reset!"); // DEBUG
				can_dash = true;
			}
		}

		// handle player's animatedsprite
		if(is_jumping) {
			sprite.Play("jump");
		}
		else if(is_dashing) {
			sprite.Play("idle"); // TODO: add a dash animation
		}
		else if(input_dir.X != 0 && is_grounded) {
			sprite.Play("walk");
		}
		else if(can_climb && input_dir.Y != 0) {
			is_climbing = true; // TODO: if I add climbing need to add a climbing animation
			sprite.Play("idle");
		}
		else if(!is_grounded) {
			// TODO: add jumping animation
			sprite.Play("jump");
		}
		else {
			sprite.Play("idle");
		}
		
		Velocity = velocity;
	}

	private void HandleJump(Godot.Vector2 input_dir) {
		Godot.Vector2 velocity = Velocity; // tmp variable to make calculations easier 

		int dir = (int)input_dir.X;
		// int wall_dir =  // Get wall direction

		if(WallJumpCheck(dir)) { // handle walljump
			// can_dash = true; // player can dash after touching a wall
			if(Input.IsActionJustPressed("jump")) {
				is_walljumping = true;
				velocity = WallJump(dir);
				// Velocity = velocity;
				// return;
			}
		}
		else if (is_grounded) { // handle regular jump
			has_fastfell = false;
			if (Input.IsActionJustPressed("jump")) {
				is_dashing = false; // dash is jump cancellable
				is_jumping = true;
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

	private Godot.Vector2 HorDash(int dir) { // dash in the direction <dir>
		Godot.Vector2 velocity = Velocity;

		is_dashing = true;
		curr_dash_time = Time.GetTicksMsec();
		if(is_grounded) {
			velocity.X = dashspeed * dir;
			dash_is_airdash = false;
		}
		else {
			velocity.X = air_dashspeed * dir;
			dash_is_airdash = true;
		}

		return velocity;
	}

	private bool WallJumpCheck(int dir) { // TODO: fix to make it more fluid
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(this.Position, this.Position + new Godot.Vector2(dir * -7, 0));
		var result = spaceState.IntersectRay(query);
		
		if( result.Count > 0 ) {
			return (ulong)result["collider_id"]==GetNode<TileMap>("/root/World/TileMap").GetInstanceId() && !is_grounded;
		}
		return IsOnWallOnly();
	}

	private Godot.Vector2 WallJump(int dir) { // wall jump in the direction dir
		Godot.Vector2 velocity = Velocity;
		has_fastfell = false;
		if (Input.IsActionJustPressed("jump")) {
			velocity.Y = -high_jumpspeed; // -high_jumpspeed * 0.9
			velocity.X = -walljump_knockback * dir; // wall bounceback
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
