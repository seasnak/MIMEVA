using Godot;

namespace Mimeva.Projectiles;
public partial class SwordSwing : Area2D
{
    private HitBox hitbox;
    private AnimatedSprite2D sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print(GetChildren());
        hitbox = GetChild<HitBox>(0);
        sprite = GetChild<AnimatedSprite2D>(1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    private void SwingSword()
    {

    }

}
