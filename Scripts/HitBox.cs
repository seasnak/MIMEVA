using Godot;
using System;
// using System.Numerics;
// using System.Runtime.Intrinsics;

namespace Mimeva;
public partial class HitBox : Area2D
{

    [Export] private int damage = 1;

    [Export] private CollisionShape2D hitbox = null;
    [Export] private bool is_breakable = false;
    public bool IsBreakable { get => is_breakable; set => is_breakable = value; }
    // private AnimatedSprite2D sprite;

    // booleans
    private bool hit_enemy = false;
    public bool HitEnemy { get => hit_enemy; set => hit_enemy = value; }

    public override void _Ready()
    {
        try
        {
            hitbox ??= GetNode<CollisionShape2D>("CollisionShape2D");
        }
        catch (Exception e)
        {
            GD.PrintErr($"{e}. Hitbox could not find Collision Shape");
        }

        // sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {

    }

    public int GetDamage() { return damage; }
    public void SetDamage(int val) { damage = val; }
    // public AnimatedSprite2D GetSprite() { return sprite; }
    public void SetActive(bool val = true) { hitbox.Disabled = !val; }
    public bool IsActive() { return !hitbox.Disabled; }
    public void Destroy() { Owner.QueueFree(); }

}
