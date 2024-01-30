using Godot;
using System;

namespace Mimeva;
public partial class Checkpoint : RigidBody2D {

    private bool is_active = false;

    private Player player; // the target player for scope

    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        
    }

    public void SpawnPlayer(Player target) {
        target.Position = this.Position;
    }

    public void SpawnPlayer() {
        player.Position = this.Position;
    }

}