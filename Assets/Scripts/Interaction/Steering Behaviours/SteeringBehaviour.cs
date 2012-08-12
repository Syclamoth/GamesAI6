using UnityEngine;
using System.Collections;

public class SteeringBehaviour  {
	private Legs legs;
	public static Vector2 ZERO_VECTOR = new Vector2(0,0);
	public virtual Vector2 getDesiredVelocity() {
		return SteeringBehaviour.ZERO_VECTOR;
	}
	public void Init(Legs legs)
	{
		this.legs = legs;
	}
	public Legs getLegs()
	{
		return legs;
	}
}
