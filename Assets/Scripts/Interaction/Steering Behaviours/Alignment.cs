using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alignment : GroupMemberSteeringBehaviour {
	public override Vector2 getDesiredVelocity ()
	{
		
		List<GameObject> nearby = this.getNearbyTaggedObjects("Sheep",1.5f);
		
		if (nearby == null)
			return this.getLegs ().getVelocity();
		
		IEnumerator<GameObject> it = nearby.GetEnumerator();
		
		Vector2 avg = new Vector2(0,0);
		
		while (it.MoveNext ())
		{
			Brain brain = (Brain)it.Current.GetComponent("Brain");
			avg += brain.legs.getVelocity();
		}
		avg /= nearby.Count;
		
		return avg;
	}
}
