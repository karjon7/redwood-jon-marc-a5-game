using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
    [ExportGroup("Camera")]
    [Export]
    private int offsetAmplify = 20;

    [ExportGroup("Rotation")]
    [Export]
    private int playerRotationSpeed = 15;

    [ExportGroup("Movement")]
    [Export]
    private int playerSpeed = 10;
    [Export]
    private int playerAccel = 15;

    [ExportGroup("Required Nodes")]
    [Export]
    public Camera2D Camera;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleCamera();
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleRotation(delta);
        HandleMovement(delta);
    }

    private void HandleCamera()
    {
        int windowWidth = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
        int windowHeight = (int)ProjectSettings.GetSetting("display/window/size/viewport_height");
        Vector2 mousePos = GetGlobalMousePosition();

        float horizontalOffset = (mousePos.X - GlobalPosition.X) / (windowWidth / 2);
        float verticalOffset = (mousePos.Y - GlobalPosition.Y) / (windowHeight / 2);

        Camera.Offset = new Vector2(horizontalOffset, verticalOffset) * offsetAmplify;
        GD.Print(Camera.Offset);
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
