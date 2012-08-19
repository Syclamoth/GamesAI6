using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alignment : GroupMemberSteeringBehaviour {
	
	public Alignment(SensableObjects allObjects) : base(allObjects) {}
	
	public override Vector2 _getDesiredVelocity ()
	{
		
		List<GameObject> nearby = this.getNearbyFilteredObjects(10.0f, AgentClassification.Sheep);
		
		if (nearby == null)
			return this.getLegs ().getVelocity();
		
		IEnumerator<GameObject> it = nearby.GetEnumerator();
		
		Vector2 avg = new Vector2(0,0);
		
		while (it.MoveNext ())
		{
			Brain brain = it.Current.GetComponent<Brain>();
			avg += brain.legs.getVelocity();
		}
		avg /= nearby.Count;
		
		return avg;
	}
}
