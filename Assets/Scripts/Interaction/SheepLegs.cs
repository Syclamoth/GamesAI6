using UnityEngine;
using System.Collections;

public class SheepLegs : Legs {
	
	public SensableObjects objectRegistry;
	
	private Arrive seekBehaviour;
	private Separation separation;
	private Cohesion cohesion;
	private Alignment alignment;
	private RandomWalk random;
	public override void init ()
	{
		if(objectRegistry) {
			objectRegistry.RegisterObject(gameObject);
		}
		seekBehaviour = new Arrive();
		seekBehaviour.Init (this);
		separation = new Separation();
		separation.Init (this);
		cohesion = new Cohesion();
		cohesion.Init (this);
		alignment = new Alignment();
		alignment.Init (this);
		random = new RandomWalk(0.6f, 1);
		random.Init (this);
		// By name
    	var go = GameObject.Find("SheepTarget");
		seekBehaviour.setTarget (go);
		this.addSteeringBehaviour(seekBehaviour);
		this.addSteeringBehaviour(separation);
		this.addSteeringBehaviour(cohesion);
		this.addSteeringBehaviour(alignment);
		this.addSteeringBehaviour(random);
	}
    public void translate(Vector2 offset)
    {
        movement.Translate(offset);
    }

    public void rotate(float offset)
    {
        movement.Rotate(offset);
    }
}
