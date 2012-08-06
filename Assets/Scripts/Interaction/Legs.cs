using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour {
	
	// Configuration data. Set this up in the inspector!
	public float maxStam = 10;
	
	
	// Runtime data. Will never be saved.
	private float currentStam;
	
	protected MoveManager movement;
	
	
	// Initialisation functions
	virtual public void Start()
	{
		movement = new MoveManager(transform);
	}
	
	// Core functions
}
