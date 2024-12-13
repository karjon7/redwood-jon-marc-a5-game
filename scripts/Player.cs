using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
    /* 
     * Upgrades 
     * (i know this could of been way better if i used an upgrade class 
     * but im already way too deep into this how it is 
     * and i only got like 5 hours to hand it in
     * so im just gonna roll with it)
    */
    public enum UpgradeType
    {
        MaxHealth,
        BulletDamage,
        BulletPierce,
        BulletRicochet,
        BulletSpeed,
        Automatic,
        MaxAmmo,
        ReloadSpeed,
        BulletsPerMin,
    }

    // Upgrade Weights
    private Dictionary<UpgradeType, int> UpgradeWeights = new Dictionary<UpgradeType, int>
    {
        { UpgradeType.MaxHealth, 5 },
        { UpgradeType.BulletDamage, 25 },
        { UpgradeType.BulletPierce, 10 },
        { UpgradeType.BulletRicochet, 10 },
        { UpgradeType.BulletSpeed, 15 },
        { UpgradeType.Automatic, 50 },
        { UpgradeType.MaxAmmo, 15 },
        { UpgradeType.ReloadSpeed, 10 },
        { UpgradeType.BulletsPerMin, 20 },
    };

    // Upgrade Increments
    private Dictionary<UpgradeType, float> UpgradeIncrements = new Dictionary<UpgradeType, float>
    {
        { UpgradeType.MaxHealth, 1f },
        { UpgradeType.BulletDamage, 1f },
        { UpgradeType.BulletPierce, 1f },
        { UpgradeType.BulletRicochet, 1f },
        { UpgradeType.BulletSpeed, 100f },
        { UpgradeType.Automatic, 1f },
        { UpgradeType.MaxAmmo, 5f },
        { UpgradeType.ReloadSpeed, 0.25f },
        { UpgradeType.BulletsPerMin, 10f },
    };

    // Upgrade Maxes
    private Dictionary<UpgradeType, float> UpgradeMaxes = new Dictionary<UpgradeType, float>
    {
        { UpgradeType.BulletSpeed, 2000f },
        { UpgradeType.MaxAmmo, 100f },
        { UpgradeType.ReloadSpeed, 0.05f },
        { UpgradeType.BulletsPerMin, 1000f },
    };

    [Export]
    public bool canShoot = true;

    [ExportGroup("Upgrades")]
    [Export]
    public int UpgradePoints = 0;
    [Export] 
    public int XP = 0;
    [Export]
    public int XPLevel = 1;

    [Export]
    private UpgradeType upgrade1;
    [Export]
    private UpgradeType upgrade2;
    [Export]
    private UpgradeType upgrade3;

    [ExportGroup("Health")]
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
    private Timer healthTimer;
    [Export]
    public Camera2D Camera;
    [Export]
    public Gun Gun;

    [ExportSubgroup("UI")]
    [Export]
    private Button upgradeButton1;
    [Export]
    private Button upgradeButton2;
    [Export]
    private Button upgradeButton3;
    [Export]
    private Label upgradePointsLabel;
    [Export]
    private ProgressBar xpProgressBar;
    [Export]
    private Label healthLabel;
    [Export]
    private Label ammoLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Health = MaxHealth; // Initialize with max health

        UpdateXPProgressBar(0.001f);
        RollUpgrades();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("reload"))
        {
            Gun.Reload();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleUI();
        HandleCamera(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleShooting();
        HandleRotation(delta);
        HandleMovement(delta);
    }

    // Upgrades
    public void RollUpgrades()
    {
        int loopAttempts = 0;
        
        // Loop runs forever unless all three upgrades are not maxed and no duplicates, or loop runs 100 times
        while (true)
        {
            upgrade1 = GetRandomUpgrade();
            upgrade2 = GetRandomUpgrade();
            upgrade3 = GetRandomUpgrade();

            if (!(IsUpgradeMax(upgrade1) || IsUpgradeMax(upgrade2) || IsUpgradeMax(upgrade3))
                && upgrade1 != upgrade2 && upgrade2 != upgrade3 && upgrade3 != upgrade1) break;

            if (loopAttempts >= 100) break;
            
            loopAttempts++;
        }
    }

    public void UpgradeChosen(int upgradePos)
    {
        switch (upgradePos)
        {
            case 1:
                Upgrade(upgrade1);
                break;

            case 2: 
                Upgrade(upgrade2);
                break;

            case 3:
                Upgrade(upgrade3);
                break;
        }
    }

    public void Upgrade(UpgradeType upgrade)
    {
        if (UpgradePoints < 1) return;

        RollUpgrades();
        UpgradePoints--;
        
        switch (upgrade)
        {
            case UpgradeType.MaxHealth:
                MaxHealth += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.BulletDamage:
                Gun.BulletDamage += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.BulletPierce:
                Gun.BulletPiercesLeft += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.BulletRicochet:
                Gun.BulletRicochetsLeft += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.BulletSpeed:
                Gun.BulletSpeed += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.Automatic:
                Gun.IsAuto = true;
                break;

            case UpgradeType.MaxAmmo:
                Gun.MaxAmmo += (int)UpgradeIncrements[upgrade];
                break;

            case UpgradeType.ReloadSpeed:
                Gun.ReloadSpeedSeconds -= UpgradeIncrements[upgrade];
                break;

            case UpgradeType.BulletsPerMin:
                Gun.BulletsPerMin += (int)UpgradeIncrements[upgrade];
                break;
        }
    }

    private bool IsUpgradeMax(UpgradeType upgrade)
    {
        // If upgrade has a specified case in the switch statement, evaluate if values equal their maxes(or mins) and return result
        // otherwise return false
        switch (upgrade)
        {
            case UpgradeType.BulletSpeed:
                return Gun.BulletSpeed >= (int)UpgradeMaxes[upgrade];

            case UpgradeType.Automatic:
                return Gun.IsAuto;

            case UpgradeType.MaxAmmo:
                return Gun.MaxAmmo >= (int)UpgradeMaxes[upgrade];

            case UpgradeType.ReloadSpeed:
                return Gun.ReloadSpeedSeconds <= UpgradeMaxes[upgrade];

            case UpgradeType.BulletsPerMin:
                return Gun.BulletsPerMin >= (int)UpgradeMaxes[upgrade];
            
            default: 
                return false;
        }
    }

    private UpgradeType GetRandomUpgrade()
    {
        // Calculate total weights of all upgrades
        int totalWeight = 0;
        foreach (int weight in UpgradeWeights.Values) totalWeight += weight;

        // Get a random value and find corresponding upgrade
        float randomValue = GD.Randf() * totalWeight;
        int sumWeight = 0;
        foreach (UpgradeType currentUpgrade in UpgradeWeights.Keys)
        {
            sumWeight += UpgradeWeights[currentUpgrade];
            if (randomValue < sumWeight) return currentUpgrade;
        }
            
        return UpgradeType.MaxHealth; // Fall back to a default upgrade if weights are invalid
        
    }

    private string GetUpgradeText(UpgradeType upgrade)
    {
        if (upgrade == UpgradeType.Automatic) return "Automatic Gun";

        string enumName = upgrade.ToString();
        return $"Increase {enumName}";
    }

    // XP
    public void AddXP(int value)
    {
        XP += value;
        CheckLevelUp();

        UpdateXPProgressBar();
    }

    public int XPToLevelUp()
    {
        return XPLevel * 5;
    }

    private void CheckLevelUp()
    {
        int xpRequired = XPToLevelUp();

        while (XP >= xpRequired)
        {
            XP -= xpRequired;
            XPLevel++;
            UpgradePoints++;
            xpRequired = XPToLevelUp();
        }
    }

    // Health
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
    }

    public void Damage(int amount)
    {
        // Subtract amount and ensure never goes below 0 (can't have negative health)
        Health -= amount;
        Health = Mathf.Max(Health, 0);

        // Start regen timer
        healthTimer.Start(3);

        // If health is 0, kill player
        if (Health == 0) Kill();
    }

    public void Kill()
    {
        // For now, when player dies, restart the scene
        GetTree().ReloadCurrentScene();
    }

    // Inputs
    private void HandleShooting()
    {
        if (!canShoot) return;
        
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


    // UI
    private void HandleUI()
    {
        HandleHUD();
    }

    private void HandleHUD()
    {
        // Upgrades
        upgradePointsLabel.SetText($"Upgrade Points: {UpgradePoints}");

        upgradeButton1.SetText(GetUpgradeText(upgrade1));
        upgradeButton2.SetText(GetUpgradeText(upgrade2));
        upgradeButton3.SetText(GetUpgradeText(upgrade3));

        
        /// Ensure you cant but an upgrade if player has no points
        if (UpgradePoints <= 0)
        {
            upgradeButton1.Disabled = true;
            upgradeButton2.Disabled = true;
            upgradeButton3.Disabled = true;
            upgradeButton1.SetTooltipText("You need Upgrade Points");
            upgradeButton2.SetTooltipText("You need Upgrade Points");
            upgradeButton3.SetTooltipText("You need Upgrade Points");
        }
        else
        {
            /// Ensure you cant buy an upgrade thats maxed
            upgradeButton1.Disabled = IsUpgradeMax(upgrade1);
            upgradeButton2.Disabled = IsUpgradeMax(upgrade2);
            upgradeButton3.Disabled = IsUpgradeMax(upgrade3);

            upgradeButton1.SetTooltipText("");
            upgradeButton2.SetTooltipText("");
            upgradeButton3.SetTooltipText("");
        }

        // Health
        healthLabel.SetText($"Health: {GetHealthState()}");
        
        // Ammo
        ammoLabel.SetText(Gun.ReloadTimer.TimeLeft == 0 ? $"Ammo: {Gun.CurrentAmmo} / {Gun.MaxAmmo}" : $"Reloading {Gun.ReloadTimer.TimeLeft:F2}");
    }

    private void UpdateXPProgressBar(float tweenSpeed = 0.75f)
    {
        // Tween to smoothly update XP progress bar
        Tween tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(xpProgressBar, "value", (float)XP / (float)XPToLevelUp(), tweenSpeed);
    }

    // Following 2 functions prevent player from shooting when clicking on the upgrade buttons
    private void ButtonHovered()
    {
        canShoot = false;
    }

    private void ButtonUnhovered()
    {
        canShoot = true;
    }
}
