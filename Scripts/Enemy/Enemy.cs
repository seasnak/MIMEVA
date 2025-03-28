using Godot;
using System;

using Mimeva.PCG;

namespace Mimeva.Entity;
public partial class Enemy : CharacterBody2D
{
    protected int max_health = 60;
    protected int curr_health = 60;

    protected int max_mana = 100;
    protected int curr_mana = 100;

    protected string enemy_type = "enemy";
    protected int enemy_value = 3;

    protected static readonly float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    protected int movespeed = 20;
    protected int max_fallspeed = (int)(gravity * 10);

    // components
    protected AnimatedSprite2D sprite;
    protected Material mat;
    protected HitBox hitbox;
    protected CollisionShape2D collider;

    // Enemy Conditionals
    private bool is_blinking_state = false;
    private bool is_dead = false;

    // Enemy Timers
    private float damage_blink_timer; // keeps track of player's health
    private float damage_blink_dur = 100; // time (in ms) for how long enemy sprite blinks

    // External Nodes
    [Export] private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        collider = GetNode<CollisionShape2D>("CollisionShape2D");
        AddToGroup("enemy");

        hitbox = GetNode<HitBox>("HitBox");
        hitbox.SetDamage(10);

        is_blinking_state = false;

        mat = sprite.Material;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {

    }

    public override void _Process(double delta)
    {
        // check death
        if (curr_health <= 0)
        {
            HandleDeath();
        }

        // handle damage blink
        if (is_blinking_state && Time.GetTicksMsec() - damage_blink_timer >= damage_blink_dur)
        {
            (mat as ShaderMaterial).SetShaderParameter("active", false);
            is_blinking_state = false;
        }
    }

    private void HandleDeath()
    {
        if (!is_dead)
        {
            is_dead = true;
            (mat as ShaderMaterial).SetShaderParameter("active", false);
            is_blinking_state = false;

            movespeed = 0;
            hitbox.SetActive(false);
            this.CollisionMask = 1;
            this.CollisionLayer = 0;

            PlayerVariables.AddEnemyKill(this.Name);
            LevelGenVariables.UpdatePlayerKillCount(1);
            sprite.Play("death");
        }
        if (!sprite.IsPlaying()) { QueueFree(); }
        // QueueFree();
    }

    protected virtual void SetMovementLogic()
    {

    }

    public virtual void Damage(int damage, bool should_blink = false)
    {
        // GD.Print($"Dealing {damage} damage to enemy");
        if (is_blinking_state) return;

        if (should_blink)
        {
            (mat as ShaderMaterial).SetShaderParameter("active", true); // activate hitflash
            damage_blink_timer = Time.GetTicksMsec();
            is_blinking_state = true;
        }
        this.curr_health -= damage;
    }

}
