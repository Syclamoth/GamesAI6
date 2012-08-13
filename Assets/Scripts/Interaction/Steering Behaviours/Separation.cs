using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Separation : GroupMemberSteeringBehaviour {

	public override Vector2 _getDesiredVelocity ()
	{
		Vector2 position = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		float avoidanceDistance = 1.5f;
		List<GameObject> objectsToAvoid = this.getNearbyTaggedObjects("Sheep", avoidanceDistance);
		
		if (objectsToAvoid == null)
			return this.getLegs ().getVelocity();
		
		Vector2 sum = new Vector2(0,0);
		
		foreach(GameObject obj in objectsToAvoid)
		{
			Vector2 target = new Vector2(obj.transform.position.x, obj.transform.position.z);
			Vector2 change = position - target;
			//sum += change.normalized * (change.magnitude / 1.5f);
			sum += change * Mathf.Lerp (500, 0, change.magnitude / avoidanceDistance);
		}
		return sum/objectsToAvoid.Count;
	}
}
