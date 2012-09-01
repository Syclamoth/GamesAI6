using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain : MonoBehaviour {

	public SensoryCortex senses;
	public Legs legs;
	public State behaviour;
	
	public AgentClassification classification;
	public SensableObjects allObjects;
	
	public BoxManager boxes;
	
	public Memory memory = new Memory();
	
	private GameObject terrainBase;
	
	public Grid levelGrid = null;

	void Start() {
		if(allObjects == null) {
			return;
		}
		//Don't give a toss about best practices frankly.
		terrainBase = GameObject.Find("TerrainBase");
		if (terrainBase != null) {
			levelGrid = terrainBase.GetComponent<Grid>();
		}
		allObjects.RegisterObject(new SensableObject(gameObject, classification));
		if(behaviour) {
			StartCoroutine(RunStateMachine());
		}
	}
	
	public void Init(BoxManager boxes, SensableObjects objs) {
		this.boxes = boxes;
		this.allObjects = objs;
		//allObjects.RegisterObject(new SensableObject(gameObject, classification));
		//if(behaviour) {
		//	StartCoroutine(RunStateMachine());
		//}
		
	}
	
	IEnumerator RunStateMachine() {
		yield return StartCoroutine(behaviour.Enter(null, this));
		while(true) {
			if (behaviour == null)
				break;
			yield return StartCoroutine(behaviour.Run (this));
		}
		//Debug.Log ("Machine stopped");
	}
	
	void RunDebug() {
		foreach(SensedObject obj in senses.GetSensedObjects())
		{
			Debug.DrawLine (transform.position, obj.getObject().transform.position, Color.blue);
		}
	}
}
