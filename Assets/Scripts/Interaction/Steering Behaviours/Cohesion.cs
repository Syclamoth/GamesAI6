using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Cohesion Steering Behaviour
 * Steers to the average location of all objects within a radius
 */
public class Cohesion : GroupMemberSteeringBehaviour {
	
	public Cohesion(SensableObjects allObjects) : base(allObjects) {}
	
	private Seek seek;
	private Seek getSeekBehaviour() {
		if (seek == null)
		{
			seek = new Seek();
			seek.Init (this.getLegs ());
		}
		return seek;
	}
	public override Vector2 _getDesiredVelocity ()
	{
		List<GameObject> nearby = this.getNearbyFilteredObjects(5.0f, AgentClassification.Sheep);
		
		if (nearby == null || nearby.Count == 0)
			return this.getLegs ().getVelocity();
		
		this.setInternalWeight(Mathf.Clamp(nearby.Count / 2, 0, 3));
		
		IEnumerator<GameObject> it = nearby.GetEnumerator();
		
		Vector2 avg = new Vector2(0,0);
		
		while (it.MoveNext ())
		{
			Vector2 target = new Vector2(it.Current.transform.position.x,it.Current.transform.position.z);
			avg += target;
		}
		avg /= nearby.Count;
		
		this.getSeekBehaviour().setTarget (avg);
		return this.getSeekBehaviour().getDesiredVelocity();
	}
}
