using Godot;
using System;

namespace Mimeva;
public partial class HurtBox : Area2D
{

	private CollisionShape2D hurtbox;

	public override void _Ready() {
		
		this.CollisionLayer = 0;
		this.CollisionMask = 2;

		hurtbox = GetNode<CollisionShape2D>("CollisionShape2D");
		hurtbox.DebugColor = new Color(1, 0, 0, 1); // set hurtbox color to red

		AreaEntered += OnAreaEntered; // connect signal for hurtbox collision detection
		
	}

    public override void _Process(double delta) {
		
    }

	public void OnAreaEntered(Area2D hitbox) {
		if (hitbox == null || hitbox is not HitBox) { return; }
		
		if(Owner.HasMethod("Damage")) {
			((Enemy)Owner).Damage( ((HitBox)hitbox).GetDamage() );
		}

	}

}
