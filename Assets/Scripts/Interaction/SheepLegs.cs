using UnityEngine;
using System.Collections;

public class SheepLegs : Legs {
	private Arrive seekBehaviour;
	public override void init ()
	{
		seekBehaviour = new Arrive();
		seekBehaviour.Init (this);
		// By name
    	var go = GameObject.Find("Player");
		seekBehaviour.setTarget (go);
		this.addSteeringBehaviour(seekBehaviour);
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
