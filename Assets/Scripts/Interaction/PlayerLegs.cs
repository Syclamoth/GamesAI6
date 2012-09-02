using UnityEngine;
using System.Collections;

public class PlayerLegs : Legs
{
	public void translate (Vector2 offset)
	{
		movement.Translate (offset);
	}

	public void rotate (float offset)
	{
		movement.Rotate (offset);
	}
}
