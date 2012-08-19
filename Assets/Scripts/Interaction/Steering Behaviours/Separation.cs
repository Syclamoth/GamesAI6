using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Separation : GroupMemberSteeringBehaviour {
	
	public Separation(SensableObjects allObjects) : base(allObjects) {}
	
	public override Vector2 _getDesiredVelocity ()
	{
		Vector2 position = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		float avoidanceDistance = 1.5f;
		List<GameObject> objectsToAvoid = this.getNearbyObjects(avoidanceDistance);
		if (objectsToAvoid == null || objectsToAvoid.Count == 0)
			return this.getLegs ().getVelocity();
		
		Vector2 sum = new Vector2(0,0);
		float smallestDistance = avoidanceDistance;
		foreach(GameObject obj in objectsToAvoid)
		{
			Vector2 target = new Vector2(obj.transform.position.x, obj.transform.position.z);
			Vector2 change = position - target;
			//sum += change.normalized * (change.magnitude / 1.5f);
			sum += change * Mathf.Lerp (300, 0, change.magnitude / avoidanceDistance);
			smallestDistance = smallestDistance > change.magnitude ? change.magnitude : smallestDistance;
		}
		
		//this.setInternalWeight(Mathf.Clamp(objectsToAvoid.Count / 2, 0, 6));
		this.setInternalWeight(Mathf.Lerp(10, 0, smallestDistance / avoidanceDistance));
		return sum/objectsToAvoid.Count;
	}
}
