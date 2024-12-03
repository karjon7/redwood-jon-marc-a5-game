using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[ExportGroup("Movement")]

	[Export]
	private int playerSpeed = 10;
	[Export]
	private int playerAccel = 15;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement(delta);
    }

    private void HandleMovement(double delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        Vector2 direction = inputDir.Normalized();

        Vector2 wishVelocity = direction * playerSpeed;

        float velocityX = Velocity.Lerp(wishVelocity, playerAccel * (float)delta).X;
        float velocityY = Velocity.Lerp(wishVelocity, playerAccel * (float)delta).Y;

        Velocity = new Vector2(velocityX, velocityY);
        MoveAndSlide();
    }
}
