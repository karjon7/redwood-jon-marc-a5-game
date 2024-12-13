using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    Player player;

    [Export]
    private NavigationAgent2D navAgent;
    [Export]
    private Area2D hurtArea;
    [Export]
    private Timer hurtTimer;

    [Export]
    public int Health = 10;

    [Export]
    private int speed = 250;
    [Export]
    private int accel = 30;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = (Player)GetTree().GetFirstNodeInGroup("Player");
        TargertPlayer();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!navAgent.IsTargetReached()) LookAt(navAgent.GetNextPathPosition());
        HandleMovement(delta);
    }

    public void HurtPlayer()
    {
        if (hurtArea.GetOverlappingBodies().Contains(player))
        {
            player.Damage(1);
            hurtTimer.Start();
        }
    }

    public void Damage(int amount)
    {
        // Subtract amount and ensure never goes below 0 (can't have negative health)
        Health -= amount;
        Health = Mathf.Max(Health, 0);

        // If health is 0, kill enemy
        if (Health == 0) Kill();
    }

    public void Kill()
    {
        // When enemy dies, give player XP
        Player player = (Player)GetTree().GetFirstNodeInGroup("Player");
        player.AddXP(3);
        QueueFree();
    }

    public void TargertPlayer()
    {
        SetNavTargetPosition(player.GlobalPosition);
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
