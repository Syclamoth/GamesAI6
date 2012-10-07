using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain : MonoBehaviour, IHearable {

	public SensoryCortex senses;
	public Legs legs;
	public State behaviour;
	
	public AgentClassification classification;
	public SensableObjects allObjects;
	
	public BoxManager boxes;
	
	public Memory memory = new Memory();
	
	public float volume = 30.0f;
	
	private GameObject terrainBase;
	
	public Grid levelGrid = null;
	
	private bool drawGUI = false;
	private string curStateDescription;
	
	
	void Start() {
		if(allObjects == null) {
			return;
		}
		//Don't give a toss about best practices frankly.
		terrainBase = GameObject.Find("TerrainBase");
		if (terrainBase != null) {
			levelGrid = terrainBase.GetComponent<Grid>();
		}
		allObjects.RegisterObject(new SensableObject(gameObject, classification), this);
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
	
	public Volume getVolume() {
		return Volume.fromDecibels(volume);
	}
	public GameObject getGameObject() {
		return gameObject;
	}
	public Vector2 getLocation() {
		return new Vector2(transform.position.x, transform.position.z);
	}
	
	public AgentClassification getClassification() {
		return classification;
	}
	
	public void ImplantMemory(MemoryEntry input) {
		memory.SetValue(input);
	}
	
	void EnableGUI(bool setEnabled) {
		drawGUI = setEnabled;
	}
	
	void OnGUI() {
		if(drawGUI) {
			GUILayout.Label("Name: " + gameObject.name);
			float panicLevel = memory.GetValue<float>("Panic");
			if(panicLevel > 0) {
				GUILayout.Label("Panic Level: " + panicLevel);
			}
			float cowardice = memory.GetValue<float>("cowardLevel");
			if(cowardice > 0) {
				GUILayout.Label("Cowardice: " + cowardice);
			}
			legs.InspectSteering();
			
			GUILayout.Label("Current State: " + behaviour.GetNiceName());
		}
	}
}
