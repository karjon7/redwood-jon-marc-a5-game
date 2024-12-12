using Godot;
using System;

public partial class Gun : Node2D
{
    [ExportGroup("Bullet")]
    [Export]
    public int BulletDamage = 1;
    [Export]
    public int BulletPiercesLeft = 0;
    [Export]
    public int BulletRicochetsLeft = 0;
    [Export]
    public float BulletSpeed = 100f;

    [ExportGroup("Gun")]
    [Export]
    public bool IsAuto = false;
    [Export]
    public int MaxAmmo = 10;
    [Export]
    public float ReloadSpeedSeconds = 5f;
    [Export]
    public float BulletsPerMin = 60f;

    [ExportGroup("Required")]
    [Export]
    public Timer ReloadTimer; 
    [Export]
    private Timer fireRateTimer; 
    [Export]
    private PackedScene bulletScene;

    public int CurrentAmmo = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CurrentAmmo = MaxAmmo; // Start Gun with full ammo
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

        if (ReloadTimer.TimeLeft > 0) return;
        if (fireRateTimer.TimeLeft > 0) return;
        if (CurrentAmmo <= 0) return;

        // Load Bullet to memory
        Bullet bulletInstance = (Bullet)bulletScene.Instantiate();

        // Give Bullet correct transforms
        bulletInstance.GlobalPosition = GlobalPosition;
        bulletInstance.Direction = (Vector2.Right).Rotated(GlobalRotation);
        bulletInstance.LookAtBulletDirection();

        // Give Bullet Stats
        bulletInstance.Damage = BulletDamage;
        bulletInstance.PiercesLeft = BulletPiercesLeft;
        bulletInstance.RicochetsLeft = BulletRicochetsLeft;
        bulletInstance.Speed = BulletSpeed;

        AddChild(bulletInstance);
        CurrentAmmo--;
        fireRateTimer.Start(60 / BulletsPerMin);
    }

    public void Reload()
    {
        if (ReloadTimer.TimeLeft > 0) return;
        if (CurrentAmmo >= MaxAmmo) return;

        // Start reload and set ammo back to max
        ReloadTimer.Start(ReloadSpeedSeconds);
        CurrentAmmo = MaxAmmo;
    }
}
