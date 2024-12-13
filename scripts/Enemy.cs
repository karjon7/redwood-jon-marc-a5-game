using Godot;
using System;

public partial class Enemy : CharacterBody2D
{

    [Export]
    private NavigationAgent2D navAgent;

    [Export]
    private int speed = 250;
    [Export]
    private int accel = 30;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetNavTargetPosition(Vector2.Zero);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!navAgent.IsTargetReached()) LookAt(Velocity);
        HandleMovement(delta);
    }

    public void SetNavTargetPosition(Vector2 targetPos) 
    {
        navAgent.TargetPosition = targetPos;
    }

    private void HandleMovement(double delta)
    {
        Vector2 nextPosistion = navAgent.GetNextPathPosition();
        Vector2 direction = GlobalPosition.DirectionTo(nextPosistion);
        Vector2 wishVelocity = direction * speed;

        wishVelocity = !navAgent.IsTargetReached() ? wishVelocity : Vector2.Zero;

        float velocityX = Velocity.Lerp(wishVelocity, accel * (float)delta).X;
        float velocityY = Velocity.Lerp(wishVelocity, accel * (float)delta).Y;

        Velocity = new Vector2(velocityX, velocityY);
        MoveAndSlide();
    }
}
