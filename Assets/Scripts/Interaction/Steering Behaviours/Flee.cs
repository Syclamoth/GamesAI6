using UnityEngine;
using System.Collections;

/*
 * Flee Steering Behaviour
 * steers away from a location
 * does not slow down to arrive, will make it look like it's orbiting
 */
public class Flee : TargetableSteeringBehaviour {
	public override Vector2 _getDesiredVelocity() {
		Vector2 target = this.getTarget ();
		if (target == default(Vector2))
		{
			this.setInternalWeight(0.0f);
			return Vector2.zero;
		}
		Vector2 myPos = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		return (myPos - target).normalized * this.getLegs().equilibrium;
	}
}
