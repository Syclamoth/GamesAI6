using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cohesion : GroupMemberSteeringBehaviour {
	private Seek seek;
	private Seek getSeekBehaviour() {
		if (seek == null)
		{
			seek = new Seek();
			seek.Init (this.getLegs ());
		}
		return seek;
	}
	public override Vector2 getDesiredVelocity ()
	{
		
		List<GameObject> nearby = this.getNearbyTaggedObjects("Sheep",1.5f);
		
		if (nearby == null)
			return this.getLegs ().getVelocity();
		
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
