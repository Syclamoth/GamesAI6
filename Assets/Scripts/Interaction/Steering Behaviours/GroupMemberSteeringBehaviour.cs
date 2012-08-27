using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupMemberSteeringBehaviour : SteeringBehaviour {
	
	private SensableObjects groupMembers;
	
	public GroupMemberSteeringBehaviour(SensableObjects allObjects) {
		groupMembers = allObjects;
	}
	
	public List<GameObject> getNearbyTaggedObjects(string tagName,float radius) {
		List<GameObject> objects = new List<GameObject>();
		GameObject[] allObjs = GameObject.FindGameObjectsWithTag(tagName);
		Vector3 myPos = this.getLegs().myTrans.position;
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
	
	public List<GameObject> getNearbyFilteredObjects(float radius, AgentClassification classification) {
		List<GameObject> objects = new List<GameObject>();
		foreach(SensableObject obj in groupMembers.GetObjectsInRadius(this.getLegs ().myTrans.position, radius)) {
			if(obj.obj == getLegs().gameObject) {
				continue;
			}
			if(obj.classification == classification) {
				objects.Add(obj.obj);
			}
		}
		return objects;
	}
	public List<GameObject> getNearbyObjects(float radius) {
		List<GameObject> objects = new List<GameObject>();
		foreach(SensableObject obj in groupMembers.GetObjectsInRadius(this.getLegs ().myTrans.position, radius)) {
			if(obj.obj == getLegs().gameObject) {
				continue;
			}
			objects.Add(obj.obj);
		}
		return objects;
	}
}
