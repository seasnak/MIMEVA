using Godot;

namespace Mimeva;
public partial class CameraController : Camera2D
{
    // Related Nodes
    [Export] CharacterBody2D player;

    // Constants
    private Godot.Vector2 DEFAULT_ZOOM = new(7.5f, 7.5f);
    private const int MAX_DISTANCE_TO_PLAYER = 8;

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

        this.Zoom = DEFAULT_ZOOM;
        this.Position = player.Position;
    }

    public override void _Process(double delta)
    {
        Vector2 dist_to_target = player.GlobalPosition - this.GlobalPosition;
        if (dist_to_target.Length() >= MAX_DISTANCE_TO_PLAYER)
        {
            this.GlobalPosition += dist_to_target.Normalized() * dist_to_target.Length() * (float)(delta) * 2f;
        }
    }

}
