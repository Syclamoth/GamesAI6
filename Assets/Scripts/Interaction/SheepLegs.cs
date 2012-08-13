using UnityEngine;
using System.Collections;

public class SheepLegs : Legs {
	private Arrive seekBehaviour;
	private Separation separation;
	private Cohesion cohesion;
	private Alignment alignment;
	public override void init ()
	{
		seekBehaviour = new Arrive();
		seekBehaviour.Init (this);
		separation = new Separation();
		separation.Init (this);
		cohesion = new Cohesion();
		cohesion.Init (this);
		alignment = new Alignment();
		alignment.Init (this);
		// By name
    	var go = GameObject.Find("SheepTarget");
		seekBehaviour.setTarget (go);
		this.addSteeringBehaviour(seekBehaviour);
		this.addSteeringBehaviour(separation);
		this.addSteeringBehaviour(cohesion);
		this.addSteeringBehaviour(alignment);
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
