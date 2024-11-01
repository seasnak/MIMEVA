using Godot;
using System;

namespace Mimeva.Projectiles;

public partial class Bullet : Area2D {

    [Export] private Sprite2D sprite;
    [Export] private int movespeed = 50;
    [Export] private int rotspeed = 10;
    [Export] private int damage = 10;
    [Export] private Node2D target;

    [Export] private uint lifespan_msec = 1500; // lifespan of bullet in msec
    private ulong spawn_time = 0;

    private Vector2 target_dir = new(0, 0);

    public int Movespeed { set => movespeed = value; get => movespeed; }
    public int Damage { get => damage; set => damage = value; }
    public Node2D Target { get => target; set => target = value; }

    public override void _Ready() {

        if(target != null) {
            target_dir = (this.GlobalPosition - target.GlobalPosition).Normalized();
        }

        spawn_time = Time.GetTicksMsec();
    }

    public override void _Process(double delta) {
        
        if(Time.GetTicksMsec() >= spawn_time + lifespan_msec) { this.QueueFree(); }

    }

    public override void _PhysicsProcess(double delta) {
        this.Position += new Vector2((float)(target_dir.X * movespeed * delta), (float)(target_dir.Y * movespeed * delta));
        this.Rotate((float)(rotspeed * delta));
    }

    public void ResetTargetAngle() {
        target_dir = -(this.GlobalPosition - target.GlobalPosition).Normalized();
        GD.Print(target_dir);
    }

}