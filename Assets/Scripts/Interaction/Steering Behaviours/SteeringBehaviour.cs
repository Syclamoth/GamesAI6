using UnityEngine;
using System.Collections;

public class SteeringBehaviour  {
	private Legs legs;
	private float weight;
	private float internal_weight;
	public static Vector2 ZERO_VECTOR = new Vector2(0,0);
	public Vector2 getDesiredVelocity() {
		internal_weight = 1.0f;
		return _getDesiredVelocity();
	}
	public virtual Vector2 _getDesiredVelocity() {
		return SteeringBehaviour.ZERO_VECTOR;
	}
	public void Init(Legs legs)
	{
		weight = 1.0f;
		internal_weight = 1.0f;
		this.legs = legs;
	}
	public Legs getLegs()
	{
		return legs;
	}
	public void setWeight(float weight) {
		this.weight = weight;
	}
	public float getWeight() {
		return weight * internal_weight;
	}
	protected void setInternalWeight(float weight)
	{
		this.internal_weight = weight;
	}
}
