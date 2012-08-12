using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain<T> : MonoBehaviour {
    public T owner;
	public SensoryCortex senses;
	public Legs legs;
	public Machine<T> behaviour;

	void Start() {
		//StartCoroutine(RunStateMachine());
        RunStateMachine();
	}
	
	void RunStateMachine() {
		while(true) {
			behaviour.Awake();
		}
	}
	
	void RunDebug() {
		foreach(SensedObject obj in senses.GetSensedObjects())
		{
			Debug.DrawLine (transform.position, obj.getObject().transform.position, Color.blue);
		}
	}
}
