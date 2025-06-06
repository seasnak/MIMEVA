using Godot;

using Mimeva.Entity;

namespace Mimeva.Component;
public partial class HurtBox : Area2D
{

    private CollisionShape2D hurtbox;

    public override void _Ready()
    {

        // this.CollisionLayer = 0;
        // this.CollisionMask = 2;

        hurtbox = GetNode<CollisionShape2D>("CollisionShape2D");
        hurtbox.DebugColor = new Color(1, 0, 0, 1); // set hurtbox color to red

        AreaEntered += OnAreaEntered; // connect signal for hurtbox collision detection

    }

    public override void _Process(double delta)
    {

    }

    public void OnAreaEntered(Area2D hitbox)
    {
        if (hitbox == null || hitbox is not HitBox) { return; }
        if (hitbox.Owner == this.Owner) { return; } // ignore if both have sample parent
                                                    // GD.Print(hitbox.Owner, this.Owner);
        if (Owner.HasMethod("Damage"))
        {
            if (Owner is Enemy enemy)
            {
                ((HitBox)hitbox).HitEnemy = true;
                enemy.Damage(((HitBox)hitbox).GetDamage(), true);
            }
            else if (Owner is Player player)
            {
                player.Damage(((HitBox)hitbox).GetDamage(), true);
            }

            if (((HitBox)hitbox).IsBreakable) { ((HitBox)hitbox).Destroy(); }
        }

    }

}
