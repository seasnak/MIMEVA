using Godot;
using System;

namespace Mimeva;
public partial class Checkpoint : Area2D {

    private Player player;
    
    private AnimatedSprite2D sprite;
    private CollisionShape2D intbox; // the area from which the player can interact with this object
    private PlayerVariables p_vars; 
    private bool player_inside = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // GD.Print("Checkpoint active");
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        player = GetNode<Player>("/root/World/Player");
        
        p_vars = GetNode<PlayerVariables>("/root/PlayerVariables");

        sprite.Play("idle");
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) 
    {
        // if(player_inside && player.IsAttacking()) {
        //     sprite.Play("ring");
        //     PlayerVariables.SetCheckpoint(this);
        // }

        if(player_inside) {
            sprite.Play("ring");
            PlayerVariables.SetCheckpoint(this);
            player_inside = false;
        }
    }

    public void SpawnPlayer(Player target) 
    {
        target.Position = this.Position;
    }

    public void SpawnPlayer()
    {
        player.Position = this.Position;
    }

    private void OnBodyEntered(Node body) 
    {
        if(body == null || body is not Player) { return; }
        
        player_inside = true;
    }

    private void OnBodyExited(Node body)
    {
        if(body == null || body is not Player) { return; }
        
        player_inside = false;
    }

}