using UnityEngine;
using System.Collections;

public class RandomWalk : SteeringBehaviour {
	// Configuration data
	private float rate; // Clamped between 0 and 1
	private float strength; // Clamped between 0 and 1
	
	// State data
	private float currentAngle;
	
	public RandomWalk(float turnRate, float turnStrength) {
		rate = Mathf.Clamp01(turnRate);
		strength = Mathf.Clamp01(turnStrength);
	}
	
	public override Vector2 _getDesiredVelocity() {
		currentAngle += Random.Range(-1f, 1f) * rate;
		
		Vector2 circleCentre = this.getLegs().getPosition() + (this.getLegs().getVelocity().normalized * Mathf.Sqrt(2));
		Vector2 currentVectorOffset = circleCentre + (new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * strength);
		
		return this.getLegs().getVelocity() + (currentVectorOffset - this.getLegs().getPosition());
	}
}
