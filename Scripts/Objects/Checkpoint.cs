using Godot;
using System;

namespace Mimeva;
public partial class Checkpoint : Area2D {

    private Player player;
    
    private AnimatedSprite2D sprite;
    private CollisionShape2D intbox; // the area from which the player can interact with this object
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // GD.Print("Checkpoint active");
        BodyEntered += OnBodyEntered;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) 
    {
        
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

        if( ((Player)body).IsAttacking() ) {
            
        }
        
    }

}