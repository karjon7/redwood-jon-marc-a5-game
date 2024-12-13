using Godot;
using System;

public partial class Game : Node
{
	[Export]
	private Node2D SpawnPointsHolder;

	[Export]
	private PackedScene playerScene;
    [Export]
    private PackedScene enemyScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		SpawnPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SpawnPlayer()
	{
		Player playerInstance = (Player)playerScene.Instantiate();
		playerInstance.GlobalPosition = GetSpawnPointGlobalPosition();
		AddChild(playerInstance);
	}

    public void SpawnEnemy()
    {
        Enemy enemyInstance = (Enemy)enemyScene.Instantiate();
        enemyInstance.GlobalPosition = GetSpawnPointGlobalPosition();
        AddChild(enemyInstance);
    }

    private Vector2 GetSpawnPointGlobalPosition()
    {
		Node2D randomSpawnPoint = (Node2D)SpawnPointsHolder.GetChildren().PickRandom();
        return randomSpawnPoint.GlobalPosition;
    }
}
