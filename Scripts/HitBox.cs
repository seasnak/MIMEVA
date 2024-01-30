 using Godot;
using System;

public partial class HitBox : Area2D
{

	[Export] 
	private int damage = 1;

	private CollisionShape2D hitbox;
	private AnimatedSprite2D sprite;

	public override void _Ready() {
		
		this.CollisionLayer = 2;
		this.CollisionMask = 0;

		hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
		hitbox.Disabled = true;

		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

	}

    public override void _Process(double delta) {
		
    }

	public int GetDamage() { return damage; }
	public void SetDamage(int val) { damage = val; }
	public AnimatedSprite2D GetSprite() { return sprite; }

}
