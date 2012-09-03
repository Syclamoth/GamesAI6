using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ears : Sense, IHearing {
	public double hearingThreshold = 0; /*Decibels*/
	private SoundManager soundManager;
	
	private Transform _myTrans;
	public Transform myTrans {
		get {
			if(_myTrans == null) {
				_myTrans = transform;
			}
			return _myTrans;
		}
	}
	
	void Start() {
		Init(GetComponent<Brain> ().allObjects.soundManager);
	}
	
	public void Init(SoundManager manager) {
		soundManager = manager;
		enabled = true;
	}
	/* Unity won't use this constructor the way you expect it to.
	public Ears(SoundManager soundManager) {
		this.soundManager = soundManager;
	}*/
	
	public override List<SensedObject> SensedObjects()
	{
		List<SensedObject> sensed = new List<SensedObject>();
		PriorityQueue<IHearable> queue = soundManager.getObjectsObservableBy(this);
		Debug.Log("Hearing stuff, " + queue.Count);
		while (queue.Count > 0) {
			IHearable curObj = queue.dequeue ();
			sensed.Add (new SensedObject(curObj.getGameObject(), curObj.getClassification()));
		}
		
		return sensed;
	}
	
	public Volume getHearingThreshold() {
		return Volume.fromDecibels (hearingThreshold);
	}
	
	public Vector2 getLocation() {
		return myTrans.position;
	}
}
