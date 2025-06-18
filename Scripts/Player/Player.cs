using Godot;
using System;

using Mimeva.PCG;

namespace Mimeva.Entity;
public partial class Player : CharacterBody2D
{
    // Player Stats
    [Export] private int max_health = 50;
    [Export] private int curr_health = 50;

    private int max_stamina = 100;
    private int curr_stamina = 100;

    private int num_coins = 0;
    private int num_keys = 0;

    // Movement Values
    private const int MOVESPEED = 60;
    private const int MIN_JUMPSPEED = 75;
    private const int MAX_JUMPSPEED = 150;
    private const int WALLJUMP_KNOCKBACK = 80;
    private const int FAST_FALLSPEED = 30;
    private const int DASHSPEED = 160;
    private const int AIRDASH_SPEED = 120;
    private const int MAX_FALLSPEED = 180;
    private const int SLASHJUMP_SPEED = 125;

    private const float GRAVITY_FALLSPEED_MULTIPLIER = 0.5f;

    // Player Action Durations (in msec)
    private const int ATTACK_DURATION = 200;
    private const int DASH_DURATION = 130;
    private const int DASH_LOCKOUT_DURATION = 200;
    private const int WALLJUMP_DURATION = 60;
    private const int COYOTETIME_DURATION = 130;
    private const int HITFLASH_DURATION = 100;

    // Player Action Timers
    private ulong curr_attack_time = 0;
    private ulong dash_start_time = 0;
    private ulong walljump_start_time = 0;
    private ulong last_grounded_time = 0;
    private ulong hitflash_start_time = 0;
    private double time_falling = 0f;

    // Player Boolean Checks
    private bool is_grounded = true;
    private bool is_held_jump = false;
    private bool has_fastfell = false;
    private bool can_climb = false;
    private bool is_climbing = false;
    private bool is_attacking = false;
    private bool is_dashing = false;
    private bool dash_is_airdash = false;
    private bool is_jumping = false;
    private bool is_walljumping = false;
    private bool hitflash_is_active = false;
    private bool touched_wall = false;

    private bool can_dash = true;
    private bool can_jump = true;

    private bool is_dead = false;


    // Related Nodes
    private AnimatedSprite2D sprite;
    private CollisionShape2D collider;
    private PlayerVariables player_vars;

    // Weapon Vars
    private Node2D weapon_obj;
    private HitBox weapon_hitbox;
    private AnimatedSprite2D weapon_sprite;
    public float WeaponRotationDeg { get => weapon_obj.RotationDegrees; }
    private Godot.Collections.Array<Godot.Vector2> weapon_pos_arr = new() { new Godot.Vector2(-9, 2f), new Godot.Vector2(8, 5), new Godot.Vector2(0, 12), new Godot.Vector2(1, -6) }; // 0 = left, 1 = right, 2 = down, 3 = up

    // Debug Nodes
    private Godot.Collections.Dictionary<string, Line2D> debugline_dict;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // Signals
    [Signal] public delegate void SwordClashedEventHandler();

    public override void _Ready()
    {
        sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        // sprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>("res://Sprites/Player/RobotPlayer/PlayerAnim.tres");

        player_vars = (PlayerVariables)GetNode("/root/PlayerVariables");
        sprite.ZIndex = 3;

        // set sword + hitbox
        weapon_obj = GetNode<Node2D>("Sword");

        weapon_hitbox = weapon_obj.GetNode<HitBox>("Hitbox");
        weapon_hitbox.SetDamage(30);
        weapon_hitbox.SetActive(false);

        weapon_sprite = weapon_obj.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        weapon_sprite.Frame = 4;
        weapon_sprite.ZIndex = 3;
        collider = GetNode<CollisionShape2D>("CollisionShape2D");
        debugline_dict = new Godot.Collections.Dictionary<string, Line2D>();

        PlayerVariables.SetPlayerStartingPos(this.Position);
    }

    public override void _PhysicsProcess(double delta)
    {

        if (this.is_dead) { return; }

        if (IsOnFloor())
        {
            is_jumping = false;
            is_walljumping = false;
            last_grounded_time = Time.GetTicksMsec();
        }
        else if (is_dashing)
        {
            Velocity = new(Velocity.X, 0);
        }
        else if (Velocity.Y < MAX_FALLSPEED)
        {
            Velocity += new Godot.Vector2(0, (float)(gravity * GRAVITY_FALLSPEED_MULTIPLIER * delta));
        }
        else
        {
            Velocity = new(Velocity.X, Math.Min(Velocity.Y, MAX_FALLSPEED));
        }

        if (!is_grounded && !touched_wall)
        {
            time_falling += delta;
            const int MAX_FALLDURATION = 2;
            if (time_falling > MAX_FALLDURATION)
            {
                // player freefall safety net
                time_falling = 0;
                curr_health = 0;
            }
        }
        else { time_falling = 0; }

        is_grounded = IsOnFloor();

        if (weapon_hitbox.IsActive() && weapon_hitbox.HitEnemy && weapon_obj.RotationDegrees == 90f)
        {
            GD.Print("Player Bounced!");
            Velocity = new Vector2(0f, -SLASHJUMP_SPEED);
            weapon_hitbox.HitEnemy = false;
        }

        Godot.Vector2 input_dir = new(
                Input.GetAxis("Left", "Right"),
                Input.GetAxis("Up", "Down")
        );
        HandleMove(input_dir, (float)delta);
        HandleJump(input_dir);
        HandleAttack(input_dir);

        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        PlayerVariables.NumCoins = num_coins;

        if (curr_health <= 0)
        {
            if (!is_dead)
            {
                sprite.Play("death");
                is_dead = true;
                PlayerVariables.NumDeaths += 1;
            }
            else if (!sprite.IsPlaying())
            {
                this.Die();
                is_dead = false;
            }
        }

        if (hitflash_is_active && ActionTimerCompleted(hitflash_start_time, HITFLASH_DURATION))
        {
            hitflash_is_active = false;
            (sprite.Material as ShaderMaterial).SetShaderParameter("active", false); // deactivate hitflash
        }

    }

    private void Die()
    {

        // add death to player death map
        GD.Print("Player has died!");
        LevelGenVariables.AddNewDeath((this.GlobalPosition.X, this.GlobalPosition.Y));

        try
        {
            this.GlobalPosition = PlayerVariables.GetCheckpointPos();
        }
        catch
        {
            this.Position = PlayerVariables.GetPlayerStartingPos();
        }

        this.Velocity = Godot.Vector2.Zero;
        curr_health = max_health;
    }

    private void HandleAttack(Godot.Vector2 input_dir)
    {

        // can't attack when dashing
        if (is_dashing) { return; }

        // handle attacking state and is_attacking
        if (is_attacking && ActionTimerCompleted(curr_attack_time, ATTACK_DURATION))
        {
            is_attacking = false;
            weapon_hitbox.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        }
        else if (is_attacking)
        {
            // don't modify position if already attacking
            return;
        }

        // handle sword position
        if (input_dir.Y > 0)
        {
            // case: swing down
            weapon_obj.Position = weapon_pos_arr[2];
            weapon_obj.RotationDegrees = 90f;
            weapon_sprite.FlipV = false;
        }
        else if (input_dir.Y < 0)
        {
            // case: swing up
            weapon_obj.Position = weapon_pos_arr[3];
            weapon_obj.RotationDegrees = -90f;
            weapon_sprite.FlipV = true;
        }
        else if (input_dir.X > 0)
        {
            // case: swing right
            weapon_obj.Position = weapon_pos_arr[1];
            weapon_obj.RotationDegrees = 0f;
            weapon_sprite.FlipV = false;
        }
        else if (input_dir.X < 0)
        {
            // case: swing left
            weapon_obj.Position = weapon_pos_arr[0];
            weapon_obj.RotationDegrees = 180f;
            weapon_sprite.FlipV = true;
        }
        else
        {
            // default
            weapon_obj.Position = sprite.FlipH ? weapon_pos_arr[0] : weapon_pos_arr[1];
            weapon_obj.RotationDegrees = sprite.FlipH ? 180 : 0;
            weapon_sprite.FlipV = sprite.FlipH;
        }

        if (Input.IsActionJustPressed("attack"))
        {
            weapon_hitbox.HitEnemy = false;
            weapon_hitbox.SetActive(true);

            weapon_sprite.Frame = 0;
            weapon_sprite.Play("slash");

            is_attacking = true;
            curr_attack_time = Time.GetTicksMsec();
        }
    }

    private void HandleMove(Godot.Vector2 input_dir, float delta = 1)
    {

        // handle player timers
        if (is_dashing && ActionTimerCompleted(dash_start_time, DASH_DURATION))
        {
            is_dashing = false;
        }
        if (is_walljumping && ActionTimerCompleted(walljump_start_time, WALLJUMP_DURATION))
        {
            is_walljumping = false;
        }

        // handle player movement
        Godot.Vector2 velocity = Velocity;

        // momentum
        if (velocity.X != MOVESPEED * input_dir.X)
        {
            if (is_grounded)
            {
                if (Math.Abs(velocity.X) > Math.Abs(input_dir.X * MOVESPEED))
                    velocity.X += 500 * delta * -Math.Sign(input_dir.X);
                if (input_dir.X > 0 && velocity.X > input_dir.X * MOVESPEED)
                {
                    velocity.X -= 500 * delta;
                }
                else if (input_dir.X < 0 && velocity.X < input_dir.X * MOVESPEED)
                {
                    velocity.X += 500 * delta;
                }
                else
                {
                    velocity.X = input_dir.X * MOVESPEED;
                }
            }
            else if (Math.Sign(velocity.X) != input_dir.X)
            {
                velocity.X = input_dir.X * MOVESPEED;
            }
        }

        // handle character sprite and weapon
        if (input_dir.X != 0 && !is_attacking && !is_dashing)
        {
            sprite.FlipH = input_dir.X < 0;
            collider.Position = new Godot.Vector2(-0.5 * input_dir.X > 0 ? -1 : 1, collider.Position.Y);

            // weapon_sprite.FlipH = sprite.FlipH;
            // weapon_sprite.Position = new(input_dir.X > 0 ? 0 : -15, weapon_sprite.Position.Y);
            // if(input_dir.X > 0) { weapon.Position = new Godot.Vector2(1, 0); }
            // else { weapon_hitbox.Position = new Godot.Vector2(-15, 0); }
        }

        // handle dash movement
        if (!is_dashing)
        {
            if (can_dash && input_dir.X != 0 && Input.IsActionJustPressed("dash"))
            {
                is_dashing = true;
                can_dash = false;
                velocity = HorDash(Math.Sign(input_dir.X));
            }

            if (!can_dash && is_grounded && Time.GetTicksMsec() - dash_start_time > DASH_LOCKOUT_DURATION)
            { // reset dash if player is on ground and not locked out
                dash_start_time = Time.GetTicksMsec();
                // GD.Print("dash reset!"); // DEBUG
                can_dash = true;
            }
        }

        // handle player's animatedsprite
        if (is_jumping)
        {
            sprite.Play("jump");
        }
        else if (is_dashing)
        {
            sprite.Play("idle"); // TODO: add a dash animation
        }
        else if (input_dir.X != 0 && is_grounded)
        {
            sprite.Play("walk");
        }
        else if (can_climb && input_dir.Y != 0)
        {
            is_climbing = true; // TODO: if I add climbing need to add a climbing animation
            sprite.Play("idle");
        }
        else if (!is_grounded)
        {
            // TODO: add jumping animation
            sprite.Play("jump");
        }
        else
        {
            sprite.Play("idle");
        }

        Velocity = velocity;
    }

    private void HandleJump(Godot.Vector2 input_dir)
    {
        Godot.Vector2 velocity = Velocity; // tmp variable to make calculations easier

        int dir = (int)input_dir.X;
        /*GD.Print($"{Time.GetTicksMsec() - last_grounded_time}");*/


        if (WallJumpCheck(dir))
        { // handle walljump
          // can_dash = true; // player can dash after touching a wall
            if (Input.IsActionJustPressed("jump"))
            {
                is_walljumping = true;
                velocity = WallJump(dir);
                // Velocity = velocity;
                // return;
            }
        }
        else if (is_grounded || (!is_jumping && Time.GetTicksMsec() - last_grounded_time <= COYOTETIME_DURATION))
        { // handle regular jump
            has_fastfell = false;
            if (Input.IsActionJustPressed("jump"))
            {
                is_dashing = false; // dash is jump cancellable
                is_jumping = true;
                velocity.Y = -MAX_JUMPSPEED;
                is_held_jump = true;
            }
        }
        else
        {
            // variable jump height
            if (Input.IsActionJustReleased("jump") && velocity.Y < -MIN_JUMPSPEED)
            {
                velocity.Y = -MIN_JUMPSPEED;
                is_held_jump = false;
            }
            // steeper jump downward arc for better jump game feel
            if (!has_fastfell && velocity.Y > 0)
            {
                // this.Velocity += new Vector2(0, FAST_FALLSPEED);
                velocity.Y = FAST_FALLSPEED;
                has_fastfell = true;
            }
        }

        Velocity = velocity;
    }

    private Godot.Vector2 HorDash(int direction)
    {
        Godot.Vector2 velocity = Velocity;

        is_dashing = true;
        dash_start_time = Time.GetTicksMsec();
        if (is_grounded)
        {
            velocity.X = DASHSPEED * direction;
            dash_is_airdash = false;
        }
        else
        {
            velocity.X = AIRDASH_SPEED * direction;
            dash_is_airdash = true;
        }

        return velocity;
    }

    private bool WallJumpCheck(int direction)
    { // TODO: fix to make it more fluid
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(this.Position, this.Position + new Godot.Vector2(direction * -7, 0));
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {

            touched_wall = (ulong)result["collider_id"] == GetNode<TileMapLayer>("/root/World/TileMap/Platforms").GetInstanceId() && !is_grounded;
            return touched_wall;
        }


        return IsOnWallOnly();
    }

    private Godot.Vector2 WallJump(int dir)
    { // wall jump in the direction dir
        Godot.Vector2 velocity = Velocity;
        has_fastfell = false;
        if (Input.IsActionJustPressed("jump"))
        {
            velocity.Y = -MAX_JUMPSPEED; // -high_jumpspeed * 0.9
            velocity.X = -WALLJUMP_KNOCKBACK * dir; // wall bounceback
            is_held_jump = true;
        }

        return velocity;
    }

    public void AddCurrency(int val) { this.num_coins += val; }
    public void Damage(int val, bool should_blink = false)
    {

        if (should_blink)
        {
            hitflash_is_active = true;
            hitflash_start_time = Time.GetTicksMsec();
            (sprite.Material as ShaderMaterial).SetShaderParameter("active", true); // activate hitflash
        }

        this.curr_health -= val;
    }
    public void Heal(int val)
    {
        this.curr_health += val;
        if (this.curr_health > this.max_health) { this.curr_health = this.max_health; }
    }

    public void BoostUp(float val)
    {
        this.Velocity -= new Vector2(0f, val);
    }

    public void BoostRight(float val)
    {
        this.Velocity += new Vector2(val, 0f);
    }

    private bool ActionTimerCompleted(float start_time_msec, float target_duration_msec)
    {
        return Time.GetTicksMsec() - start_time_msec <= target_duration_msec;
    }

    // Getters and Setters
    // TODO: remove old getter setter functions
    public int NumCoins { get => this.num_coins; set => this.num_coins = value; }

    public void SetClimb(bool val) { this.can_climb = val; }
    public bool GetClimb() { return this.can_climb; }
    public bool CanClimb { get => this.can_climb; set => this.can_climb = value; }

    public int GetKeys() { return num_keys; }
    public void SetKeys(int val) { num_keys = val; }
    public int NumKeys { get => this.num_keys; set => this.num_keys = value; }
    public void AddKeys(int val) { num_keys += val; }

    public int GetCurrHealth() { return this.curr_health; }
    public void SetCurrHealth(int val) { this.curr_health = val; }
    public int CurrHealth { get => this.curr_health; set => this.curr_health = value; }

    public int GetMaxHealth() { return this.max_health; }
    public void SetMaxHealth(int val) { this.max_health = val; }
    public int MaxHealth { get => this.max_health; set => this.max_health = value; }

    public int GetCoins() { return this.num_coins; }
    public void SetCoins(int val) { this.num_coins = val; }
    public int Coins { get => this.num_coins; set => this.num_coins = value; }

    public bool GetIsAttacking() { return is_attacking; }
    public void SetIsAttacking(bool val) { is_attacking = val; }
    public bool IsAttacking { get => this.is_attacking; set => this.is_attacking = value; }
}
