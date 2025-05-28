using Godot;
using System;

namespace Mimeva;
public partial class CameraController : Camera2D
{

    [Export] CharacterBody2D player;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (player == null)
        {
            try
            {
                player = GetNode<CharacterBody2D>("/root/World/Player");
            }
            catch
            {
                GD.PrintErr("Camera could not find Player");
                throw;
            }
        }

        this.Zoom = new Vector2(7.5f, 7.5f);
        this.Position = player.Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // smooth camera
        Vector2 dist_to_target = player.GlobalPosition - this.GlobalPosition;
        if (dist_to_target.Length() > 8)
        {
            this.GlobalPosition += dist_to_target.Normalized() * dist_to_target.Length() * (float)(delta) * 2f;
            // this.GlobalTransform = player.Transform.InterpolateWith(this.Transform, 0.1f);
        }
    }

}
