using UnityEngine;
using System.Collections;

public class Seek : TargetableSteeringBehaviour {
	public override Vector2 getDesiredVelocity() {
		Vector2 target = this.getTarget ();
		if (target == default(Vector2))
			return SteeringBehaviour.ZERO_VECTOR;
		Vector2 myPos = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		return (target - myPos).normalized * this.getLegs().equilibrium;
	}
}
