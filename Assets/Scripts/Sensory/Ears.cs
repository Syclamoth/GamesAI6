using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ears : Sense, IHearing {
	public double hearingThreshold = 0; /*Decibels*/
	private SoundManager soundManager;
	public Ears(SoundManager soundManager) {
		this.soundManager = soundManager;
	}
	
	public override List<SensedObject> SensedObjects()
	{
		List<SensedObject> sensed = new List<SensedObject>();
		PriorityQueue<IHearable> queue = soundManager.getObjectsObservableBy(this);
		while (queue.Count > 0) {
			sensed.Add (new SensedObject(queue.dequeue ().getGameObject()));
		}
		return sensed;
	}
	
	public Volume getHearingThreshold() {
		return Volume.fromDecibels (hearingThreshold);
	}
	
	public Vector2 getLocation() {
		return gameObject.transform.position;
	}
}
