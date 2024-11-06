using Godot;
using System;

using Mimeva.Projectiles;

namespace Mimeva;

public partial class PinkGlorp : Glorp {

    public override void _Ready() {
        base._Ready();
    }

    public override void _PhysicsProcess(global::System.Double delta) {
        base._PhysicsProcess(delta);
    }

    protected override void SetMovementLogic() {
        Godot.Vector2 velocity = Velocity;

        if( IsOnWall() ) { movespeed = -movespeed; }
        if( this.IsOnEdge ) { movespeed = -movespeed; }
        velocity.X = movespeed;

        Velocity = velocity;        
    }





    
}