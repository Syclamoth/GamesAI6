using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Legs : MonoBehaviour {
	private List<SteeringBehaviour> steeringBehaviours = new List<SteeringBehaviour>();
	
	// Configuration data. Set this up in the inspector!
	public float maxStam = 10;
	public float mass = 15.0f; //kg
	
	public float maxSpeed = 9.0f; //m s-1
	public float maxForce = 10.0f; //Newtons
	
	/* Can run at this speed constantly without tiring.
	   any faster drains stamina, any slower regenerates
	   it. */
	public float equilibrium = 3.0f; //m s-1
	
	private Vector2 velocity = new Vector2(0,0); //m s-1
	private Vector2 acceleration = new Vector2(0,0); //m s-2
	// Runtime data. Will never be saved.
	private float currentStam;
	
	protected MoveManager movement;
	
	
	// Initialisation functions
	public void Start()
	{
		movement = new MoveManager(transform);
		this.init ();
	}
	
	virtual public void init() {
		//Empty, just to make sure stuff in Start() is not accidentally
		//overwritten
	}
	
	// Core functions
	public void addSteeringBehaviour(SteeringBehaviour behaviour)
	{
		steeringBehaviours.Add (behaviour);
	}
	
	public void Update()
	{
		if (steeringBehaviours.Count > 0)
		{
			acceleration = new Vector2(0,0);
			IEnumerator<SteeringBehaviour> it = steeringBehaviours.GetEnumerator ();
			for (it.Reset();it.MoveNext();)
			{
				//I have no idea why ClampMagnitude is static...
				Vector2 steering_force = Vector2.ClampMagnitude(it.Current.getDesiredVelocity() - velocity,maxForce);
				acceleration += steering_force/mass;
			}
			acceleration /= steeringBehaviours.Count;
			
			velocity = Vector2.ClampMagnitude(velocity + acceleration,maxSpeed);
			this.transform.position += new Vector3(velocity.x,0,velocity.y);
			//movement.Translate (velocity * Time.deltaTime);
		}
	}
}
