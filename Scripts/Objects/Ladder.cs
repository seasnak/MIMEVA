using Godot;

using Mimeva.Entity;

namespace Mimeva.Object;
public partial class Ladder : RigidBody2D
{

    private bool is_active = false;

    private Player player; // the target player for scope

    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }

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

    public void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            ((Player)body).SetClimb(true);
        }
    }

    public void OnBodyExited(Node body)
    {
        if (body is Player)
        {
            ((Player)body).SetClimb(false);
        }
    }

}
