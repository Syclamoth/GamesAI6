using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SensableObjects : MonoBehaviour {

	private HashSet<GameObject> objects = new HashSet<GameObject>();
	
	public List<GameObject> GetObjectsInRadius(Vector3 position, float radius)
	{
		// Currently implemented in the most naive fashion. Will add additional algorithms later,
		// with logic to choose the most efficient one for the current task.
		List<GameObject> retV = new List<GameObject>();
		float sqrRadius = radius * radius;
		foreach(GameObject obj in objects)
		{
			if((obj.transform.position - position).sqrMagnitude < sqrRadius)
			{
				retV.Add (obj);
			}
		}
		return retV;
	}
	
	public void RegisterObject(GameObject obj)
	{
		if(!objects.Add (obj))
		{
			Debug.Log ("Object " + obj.name + " already registered!");
		}
	}
}
