using Godot;
using Godot.Collections;
using System;

public partial class Bullet : Node2D
{	
	[Export]
	public float BulletSpeed = 20f;
	[Export]
	public int BulletDamage = 1;
	[Export]
	public int MaxDistance = 2000;
	public Vector2 BulletDirection = Vector2.Right;

    [Export(PropertyHint.Layers2DPhysics)]
    private uint collideWith;
	private Vector2 prevPos;
	private float distanceTravelled = 0f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		prevPos = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// Calculate and move bullet to new position
		float newPosX = GlobalPosition.X + (BulletDirection.X * BulletSpeed * (float)delta);
		float newPosY = GlobalPosition.Y + (BulletDirection.Y * BulletSpeed * (float)delta);

		Vector2 newPos = new Vector2(newPosX, newPosY);

        // Raycast to bullet new position and get result
        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(prevPos, newPos, collideWith);
		Dictionary result = GetWorld2D().DirectSpaceState.IntersectRay(query);

		if (result.Count > 0) // Raycast hit something
		{
			Node resultCollider = (Node)result["collider"];
			Vector2 resultPosition = (Vector2)result["position"];

            if (resultCollider.IsInGroup("Enemy")) // Raycast hit an Enemy, damage Enemy and destroy bullet
			{
				if (resultCollider.HasMethod("Damage")) resultCollider.Call("Damage", BulletDamage);
				Destroy();
			}
			else // Raycast hit terrain, move bullet to collision (for hit particles if have time) and destroy bullet
			{
				newPos = resultPosition;
				Destroy();
			}
		}

        // Add distance travelled and destroy if gone too far
        distanceTravelled += prevPos.DistanceTo(newPos);
        if (distanceTravelled >= MaxDistance) Destroy();

        // Move bullet
        GlobalPosition = newPos;
		
		// Next frame information
        prevPos = newPos;
    }

    public void Destroy()
	{
		QueueFree();
	}

}
