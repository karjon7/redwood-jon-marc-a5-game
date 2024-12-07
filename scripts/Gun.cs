using Godot;
using System;

public partial class Gun : Node2D
{
    [Export]
    private PackedScene bulletScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Shoot()
    {
        if (bulletScene == null) // No Bullet scene
        {
            GD.PrintErr("No Bullet scene assigned");
            return;
        }

        Bullet bulletInstance = (Bullet)bulletScene.Instantiate();

        bulletInstance.GlobalPosition = GlobalPosition;
        bulletInstance.BulletDirection = (Vector2.Right).Rotated(GlobalRotation);
        bulletInstance.LookAtBulletDirection();

        AddChild(bulletInstance);
    }
}
