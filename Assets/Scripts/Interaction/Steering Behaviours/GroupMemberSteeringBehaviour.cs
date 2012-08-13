using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupMemberSteeringBehaviour : SteeringBehaviour {
	public List<GameObject> getNearbyTaggedObjects(string tagName,float radius) {
		List<GameObject> objects = new List<GameObject>();
		GameObject[] allObjs = GameObject.FindGameObjectsWithTag(tagName);
		Vector3 myPos = this.getLegs().transform.position;
		for (int i=0;i < allObjs.Length;++i)
		{
			if (allObjs[i] == this.getLegs ().gameObject)
				continue;
			float magnitude = (myPos - allObjs[i].transform.position).sqrMagnitude;
			
			if (magnitude <= radius*radius) {
				objects.Add (allObjs[i]);
			}
		}
		if (objects.Count == 0)
			return null;
		return objects;
	}
}
