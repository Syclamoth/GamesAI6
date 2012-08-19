using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain : MonoBehaviour {

	public SensoryCortex senses;
	public Legs legs;
	public State behaviour;
	
	public Memory memory = new Memory();
	
	void Start() {
		if(behaviour) {
			StartCoroutine(RunStateMachine());
		}


		StartCoroutine(RunStateMachine());
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
