using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class Eyes : Sense {
    
    public float fOV = 47.5f; //degrees
    public float focusedFOV = 2.5f;
    public float peripheralFOV = 100.0f;
	
	public float maxViewDistance = 100;
	
	public float attentiveness = 0.7f;
	
	public LayerMask visibleLayers;
	private SensableObjects allObjects;
	
	private Dictionary<SensableObject, RaycastAggregate> aggregates = new Dictionary<SensableObject, RaycastAggregate>();
	
	private List<StringAtPoint> debugStrings = new List<StringAtPoint>();
	
	void Awake() {
		allObjects = GetComponent<Brain>().allObjects;
	}
	
    public override List<SensedObject> SensedObjects()
    {
		List<SensedObject> retV = new List<SensedObject>();
		debugStrings = new List<StringAtPoint>();
		float sqrRadius = maxViewDistance * maxViewDistance;
		foreach (SensableObject obj in aggregates.Keys)
		{
			if((obj.obj.transform.position - transform.position).sqrMagnitude < sqrRadius)
			{
				// The object is in range, check its angle
				if(Vector3.Angle(obj.obj.transform.position - transform.position, transform.forward) < peripheralFOV)
				{
					float totalAggregate = GetTotalAggregate(obj);
					debugStrings.Add(new StringAtPoint(totalAggregate.ToString(), obj.obj.transform.position));
					if(totalAggregate > attentiveness) {
						retV.Add(new SensedObject(obj.obj, obj.classification));
					}
				}
			}
		}
		return retV;
    }
	
	private float GetTotalAggregate(SensableObject obj)
	{
		float baseAggregate = aggregates[obj].GetAggregate(1); // This number determines how quickly the eyes can pick up new targets
		float angle = Vector3.Angle(obj.obj.transform.position - transform.position, transform.forward);
		if(angle < focusedFOV) {
			//Debug.Log ("Focused");
			return baseAggregate * 1.5f;
		}
		if(angle < fOV)
		{
			//Debug.Log ("Visible");
			return baseAggregate;
		}
		//Debug.Log ("Peripheral");
		// When I have access to speed data, put the speed modifier here. Objects should get more visible when moving fast.
		return baseAggregate * (1 - ((fOV - angle) / (fOV - peripheralFOV)));
	}
	
	public void Update()
	{
		foreach(SensableObject obj in allObjects.GetObjectsInRadius(transform.position, maxViewDistance))
		{
			if(!aggregates.ContainsKey(obj))
			{
				aggregates.Add(obj, new RaycastAggregate(transform, obj.obj.GetComponentInChildren<MeshFilter>(), visibleLayers));
			}
			aggregates[obj].QueueRaycast();
		}
		
	}/*
	//Delete this when I'm done
	void OnGUI()
	{
		foreach(StringAtPoint str in debugStrings)
		{
			Vector2 wordCentre = Camera.main.WorldToScreenPoint(str.point);
			wordCentre.y = Screen.height - wordCentre.y;
			GUI.color = Color.red;
			GUI.Label (new Rect(wordCentre.x - 20, wordCentre.y - 10, 40, 20), str.word);
		}
	}*/
}

public class RaycastAggregate {
	private Transform startTrans;
	private MeshFilter targetFilter;
	private LayerMask occludingLayers;
	
	private Queue<TimedBool> raycasts = new Queue<TimedBool>();
	
	public RaycastAggregate(Transform startTransform, MeshFilter targettedFilter, LayerMask visibleLayers)
	{
		startTrans = startTransform;
		targetFilter = targettedFilter;
		occludingLayers = visibleLayers;
	}
	
	public void QueueRaycast()
	{
		// Randomly select a point in the target mesh:
		Vector3 endPoint = targetFilter.transform.TransformPoint(targetFilter.mesh.vertices[UnityEngine.Random.Range (0, targetFilter.mesh.vertexCount)]);
		RaycastHit hit;
		// The boolean to be added to the queue is determined by the result of a raycast between the eye, and the target point.
		// queueThis is true if the object is not obstructed, and false otherwise.
		bool queueThis = !Physics.Linecast(startTrans.position, endPoint, out hit, occludingLayers);
		if(hit.transform == targetFilter.transform) {
			// If the object obstructing the raycast is actually the same object, fix the value.
			queueThis = true;
		}
		//Add a new timed boolean to the queue of timed booleans. TimeBool class
		raycasts.Enqueue(new TimedBool(queueThis));
		Debug.DrawLine(startTrans.position, endPoint, queueThis ? Color.green : Color.red);
	}
	
	public bool ContainsData()
	{
		return raycasts.Count > 0;
	}
	
	// Gets the aggregete over 'timeout' seconds of the number of raycasts which are not obstructed.
	// The value shows what percentage of the object is obstructed by other colliders.
	public float GetAggregate(float timeout)
	{
		// First, cull the queue:
		try {
			while(raycasts.Peek().ShouldRemove(timeout))
			{
				raycasts.Dequeue();
			}
		} catch (InvalidOperationException e) {
			// If the queue is empty
			return 0;
		}
		// now, get the average!
		float total = 0;
		
		foreach(TimedBool curBool in raycasts)
		{
			total += curBool.flag ? 1 : 0;
		}
		total = total / raycasts.Count;
		
		// Now, apply adjustments for small amounts of data:
		float gap = raycasts.Peek().TimeGap(raycasts.ElementAt(raycasts.Count - 1)); // VERIFY THIS
		// This artificially decreases the total if the object has not been seen for long enough.
		gap = gap / timeout;
		total = total * gap;
		return total;
	}
}

public struct TimedBool
{
	public bool flag;
	private float timeSet;
	
	public TimedBool(bool value)
	{
		flag = value;
		timeSet = Time.time;
	}
	public bool ShouldRemove(float timer)
	{
		return (Time.time - timeSet) > timer;
	}
	
	public float TimeGap(TimedBool other)
	{
		return Mathf.Abs(other.timeSet - timeSet);
	}
}

public struct StringAtPoint
{
	// Used for Gizmo debugging!
	public string word;
	public Vector3 point;
	public StringAtPoint(string newWord, Vector3 newPoint)
	{
		word = newWord;
		point = newPoint;
	}
}
