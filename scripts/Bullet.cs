using Godot;
using Godot.Collections;
using System;

public partial class Bullet : Node2D
{	
	public int Damage = 1;
	public int PiercesLeft = 0;
	public int RicochetsLeft = 0;
    public float Speed = 20f;

    [Export]
	public int MaxDistance = 2000;
	public Vector2 Direction = Vector2.Right;

    [Export(PropertyHint.Layers2DPhysics)]
    private uint collideWith;
	private Vector2 prevPos;
	private float distanceTravelled = 0f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		TopLevel = true;
		prevPos = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Calculate and move bullet to new position
		float newPosX = GlobalPosition.X + (Direction.X * Speed * (float)delta);
		float newPosY = GlobalPosition.Y + (Direction.Y * Speed * (float)delta);

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
				if (resultCollider.HasMethod("Damage")) resultCollider.Call("Damage", Damage);
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

        // Move and rotate bullet
        GlobalPosition = newPos;
		LookAtBulletDirection();
		
		// Next frame information
        prevPos = newPos;
    }

	public void LookAtBulletDirection()
	{
        LookAt(GlobalPosition + Direction);

    }

    public void Destroy()
	{
		QueueFree();
    }

}
