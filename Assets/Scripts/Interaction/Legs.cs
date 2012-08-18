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
	
	public float equilibrium = 3.0f; //m s-1
	
	public bool inspectSteering = false;
	
	private Vector2 velocity = new Vector2(0,0); //m s-1
	private Vector2 acceleration = new Vector2(0,0); //m s-2
	// Runtime data. Will never be saved.
	private float currentStam;
	
	protected MoveManager movement;
	
	
	private Transform _myTrans = null;
	
	public Transform myTrans {
		get {
			if(_myTrans == null) {
				_myTrans = transform;
			}
			return _myTrans;
		}
	}
	
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
	
	public void removeSteeringBehaviour(SteeringBehaviour behaviour)
	{
		steeringBehaviours.Remove(behaviour);
	}
	
	public void Update()
	{
		if (steeringBehaviours.Count > 0)
		{
			acceleration = new Vector2(0,0);
			float sum = 0;
			/* The foreach statement in C# is internally identical to using a for loop over the iterator returned by 'GetEnumerator',
			 * only with clearer syntax. Don't be afraid of them!
			 * */
			foreach (SteeringBehaviour behaviour in steeringBehaviours)
			{
				//I have no idea why ClampMagnitude is static...
				Vector2 steering_force = Vector2.ClampMagnitude(behaviour.getDesiredVelocity() - velocity,maxForce);
				sum += behaviour.getWeight ();
				acceleration += behaviour.getWeight()*(steering_force/mass);
				Debug.DrawRay(myTrans.position, behaviour.getDesiredVelocity().ToWorldCoords(), ColourUtility.GetSpectrum(sum));
			}
			if (sum == 0)
				return;
			acceleration /= sum;
			velocity = Vector2.ClampMagnitude(velocity + (acceleration * Time.deltaTime), maxSpeed);
			myTrans.position += new Vector3(velocity.x,0,velocity.y) * Time.deltaTime;
			if(velocity.sqrMagnitude > 0) {
				myTrans.rotation = Quaternion.LookRotation(new Vector3(velocity.x,0,velocity.y));
			}
			Debug.DrawRay(myTrans.position, new Vector3(velocity.x,0,velocity.y), Color.yellow);
			//movement.Translate (velocity * Time.deltaTime);
		}
	}
	
	public Vector2 getVelocity() {
		return velocity;
	}
	
	public Vector2 getPosition() {
		return new Vector2(myTrans.position.x, myTrans.position.y);
	}
	
	void OnGUI() {
		if(!inspectSteering) {
			return;
		}
		foreach(SteeringBehaviour behaviour in steeringBehaviours) {
			GUILayout.Label(behaviour.GetType().ToString() + ", Current velocity: " + behaviour.getDesiredVelocity() + ", Weight: " + behaviour.getWeight());
		}
	}
}
