using UnityEngine;
using System.Collections;

public class RandomWalk : SteeringBehaviour {
	// Configuration data
	private float rate; // Clamped between 0 and 1
	private float strength; // Clamped between 0 and 1
	private float forceMultiplier;
	
	// State data
	private float currentAngle;
	
	private float lastStep;
	private float stepTime = 0.1f;
	
	public RandomWalk(float turnRate, float turnStrength) {
		rate = Mathf.Clamp01(turnRate);
		strength = Mathf.Clamp01(turnStrength);
		forceMultiplier = 50;
	}
	
	public RandomWalk(float turnRate, float turnStrength, float force) {
		rate = Mathf.Clamp01(turnRate);
		strength = Mathf.Clamp01(turnStrength);
		forceMultiplier = force;
	}
	
	public override Vector2 _getDesiredVelocity() {
		if(Time.time > lastStep + stepTime) {
			currentAngle += Random.Range(-1f, 1f) * rate;
			lastStep += stepTime;
		}
		
		Vector2 circleCentre = this.getLegs().getPosition() + (this.getLegs().getVelocity().normalized * Mathf.Sqrt(2));
		Vector2 currentVectorOffset = circleCentre + (new Vector2(Mathf.Sin(currentAngle) * strength, Mathf.Cos(currentAngle)) * strength);
		
		return this.getLegs().getVelocity() + ((currentVectorOffset - this.getLegs().getPosition()) * forceMultiplier);
	}
}
