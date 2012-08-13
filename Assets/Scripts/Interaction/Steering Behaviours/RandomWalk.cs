using UnityEngine;
using System.Collections;

public class RandomWalk : SteeringBehaviour {
	private Vector2 velocityDelta = Vector2.zero;
	
	public override Vector2 _getDesiredVelocity() {
		return this.getLegs().getVelocity() + (velocityDelta * Time.deltaTime);
	}
}
