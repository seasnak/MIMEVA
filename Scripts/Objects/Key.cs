using Godot;

using Mimeva.Entity;

namespace Mimeva.Object;
public partial class Key : Area2D
{

    private AnimatedSprite2D sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnAreaEntered;

        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.SpeedScale = 1;
        sprite.Play("idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    private void OnAreaEntered(Node body)
    {
        if (body == null || body is not Player) { return; }

        ((Player)body).AddKeys(1);
        QueueFree();
    }
}
