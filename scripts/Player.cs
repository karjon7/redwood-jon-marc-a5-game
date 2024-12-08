using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Health = 10;
    [Export]
    public int MaxHealth = 10;

    [ExportGroup("Camera")]
    [Export]
    private int offsetAmplify = 20;
    [Export]
    private int offsetSmoothing = 15;

    [ExportGroup("Rotation")]
    [Export]
    private int playerRotationSpeed = 15;

    [ExportGroup("Movement")]
    [Export]
    private int playerSpeed = 10;
    [Export]
    private int playerAccel = 15;

    [ExportGroup("Required")]
    [Export]
    public Camera2D Camera;
    [Export]
    public Gun Gun;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Health = MaxHealth; // Initialize with max health
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleCamera(delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("reload"))
        {
            Gun.Reload();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleShooting();
        HandleRotation(delta);
        HandleMovement(delta);
    }

    public string GetHealthState()
    {
        // Get health percantage and get corrisponding health state
        
        float healthPercentage = (float)Health / (float)MaxHealth;

        if (healthPercentage < 0.2) return "Critical";
        else if (healthPercentage < 0.4) return "Weakened";
        else if (healthPercentage < 0.6) return "Hurt";
        else if (healthPercentage < 0.8) return "Stable";
        else return "Optimal";
    }

    public void Heal(int amount)
    {
        // Add amount and ensure never goes above max health
        Health += amount;
        Health = Mathf.Min(Health, MaxHealth);

        GD.Print(GetHealthState());
    }

    public void Damage(int amount)
    {
        // Subtract amount and ensure never goes below 0 (can't have negative health)
        Health -= amount;
        Health = Mathf.Max(Health, 0);

        // If health is 0, kill player
        if (Health == 0) Kill();

        GD.Print(GetHealthState());
    }

    public void Kill()
    {
        // For now, when player dies, restart the scene
        GetTree().ReloadCurrentScene();
    }

    private void HandleShooting()
    {
        if (!Gun.IsAuto && Input.IsActionJustPressed("primary_fire")) Gun.Shoot(); // Semi Auto
        if (Gun.IsAuto && Input.IsActionPressed("primary_fire")) Gun.Shoot(); // Auto
    }

    private void HandleCamera(double delta)
    {
        int windowWidth = DisplayServer.ScreenGetSize().X;
        int windowHeight = DisplayServer.ScreenGetSize().Y;
        Vector2 mousePos = GetGlobalMousePosition();

        float horizontalOffset = (mousePos.X - GlobalPosition.X) / (windowWidth / 2);
        float verticalOffset = (mousePos.Y - GlobalPosition.Y) / (windowHeight / 2);

        Vector2 targetOffset = new Vector2(horizontalOffset, verticalOffset) * offsetAmplify;

        Camera.Offset = Camera.Offset.Lerp(targetOffset, offsetSmoothing * (float)delta);
    }
    
    private void HandleRotation(double delta)
    {
        Vector2 directionToTarget = GlobalPosition.DirectionTo(GetGlobalMousePosition());
        float targetAngle = directionToTarget.Angle();

        GlobalRotation = Mathf.LerpAngle(GlobalRotation, targetAngle, playerRotationSpeed * (float)delta);
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
