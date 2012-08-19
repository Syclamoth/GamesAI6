using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain : MonoBehaviour {

	public SensoryCortex senses;
	public Legs legs;
	public State behaviour;
	
	public Memory memory = new Memory();
	
	void Start() {
        System.Random ranObj = new System.Random();

        //set courageLevel for sheep
        memory.SetValue("courageLevel", (float)ranObj.NextDouble());

        //set leaderLevel for wolf
        memory.SetValue("leaderLevel", (float)ranObj.Next(100));

        if (gameObject.tag == "Sheep" || gameObject.name == "Sheep")
        {
            legs = new SheepLegs();
        }
		StartCoroutine(RunStateMachine());
	}
	
	IEnumerator RunStateMachine() {
		while(true) {
			if (behaviour == null)
				break;
			yield return behaviour.Run (this);
		}
	}
	
	void RunDebug() {
		foreach(SensedObject obj in senses.GetSensedObjects())
		{
			Debug.DrawLine (transform.position, obj.getObject().transform.position, Color.blue);
		}
	}
}
