using UnityEngine;
using System.Collections;

/* Arrive Steering Behaviour
 * Steers towards a certain target point, and then slows down as it gets there.
 */

public class Arrive : TargetableSteeringBehaviour {

	public override Vector2 _getDesiredVelocity ()
	{
		Vector2 position = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		Vector2 target = this.getTarget ();
		if (target == default(Vector2))
		{
			this.setInternalWeight(0.0f);
			return this.getLegs ().getVelocity();
		}
		
		Vector2 target_offset = target - position;
		//if (target_offset.sqrMagnitude < 9.0f)
		//	return SteeringBehaviour.ZERO_VECTOR;
    	float distance = target_offset.magnitude;
    	float ramped_speed = this.getLegs().equilibrium * (distance / 2.0f);
    	float clipped_speed = System.Math.Min(ramped_speed,this.getLegs().equilibrium);
    	return (clipped_speed / distance) * target_offset * 10;
	}
}
