using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Separation : GroupMemberSteeringBehaviour {

	public override Vector2 _getDesiredVelocity ()
	{
		Vector2 position = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		
		List<GameObject> objectsToAvoid = this.getNearbyTaggedObjects("Sheep",1.5f);
		
		if (objectsToAvoid == null)
			return this.getLegs ().getVelocity();
		
		IEnumerator<GameObject> it = objectsToAvoid.GetEnumerator();
		
		Vector2 sum = new Vector2(0,0);
		
		while (it.MoveNext ())
		{
			Vector2 target = new Vector2(it.Current.transform.position.x,it.Current.transform.position.z);
			Vector2 change = position - target;
			sum += change.normalized * (change.magnitude / 1.5f);
		}
		return sum/objectsToAvoid.Count;
	}
}
