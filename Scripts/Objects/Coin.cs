using Godot;
/*using System;*/
/*using System.Collections;*/

namespace Mimeva;
public partial class Coin : Area2D
{

    private AnimatedSprite2D sprite;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.SpeedScale = 2;
        sprite.ZIndex = 2;
        sprite.Play("idle");
    }

    public override void _Process(double delta)
    {

    }

    private void OnBodyEntered(Node body)
    {
        if (body == null || body is not Player) { return; }

        ((Player)body).AddCurrency(1);
        QueueFree();
    }

}
