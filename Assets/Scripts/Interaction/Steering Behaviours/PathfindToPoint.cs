using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindToPoint : TargetableSteeringBehaviour {
	private Grid grid;
	
	private Arrive auxiliarySteering;
	
	private LinkedList<GridSquare> path;
	private LinkedListNode<GridSquare> current;
	public void Init(Legs legs,Grid grid) {
		base.Init(legs);
		this.grid = grid;
		this.auxiliarySteering = new Arrive();
		this.auxiliarySteering.Init(getLegs());
	}
	
	public override void onTarget ()
	{
		Vector2 target2 = getTarget ();
		Vector3 target = new Vector3(target2.x,0,target2.y);
		
		path = grid.findPath (this.getLegs().transform.position,target);
		
		if (path != null)
		{
			current = path.First;
			LayerMask mask = new LayerMask();
			mask.AddToMask("Buildings");
			while(current != null && !Physics.Linecast(getLegs().transform.position, current.Value.toVector3()))
			{
				//Debug.Log("BLAH");
				//Debug.DrawLine(getLegs().transform.position, current.Value.toVector3(), Color.green);
				current = current.Next;
			}
		} else {
			current = null;
		}
	}
	
	public override Vector2 _getDesiredVelocity() {
		onTarget ();
		
		if (current == null)
		{
			//Debug.Log ("Using aux steering!");
			auxiliarySteering.setTarget(this.getTarget());
			return auxiliarySteering.getDesiredVelocity();
		}
		
		if (grid.gridSquareFromVector3(this.getLegs ().transform.position) == current.Value) {
			if (current.Next == null) {
				current = null;
				this.setInternalWeight(1.0f);
				return Vector2.zero;
			}
			current = current.Next;
			LayerMask mask = new LayerMask();
			mask.AddToMask("Buildings");
			while(current != null && !Physics.Linecast(getLegs().transform.position, current.Value.toVector3()))
			{
				if (current.Next == null) {
					current = null;
					this.setInternalWeight(1.0f);
					return Vector2.zero;
				}
				current = current.Next;
			}
		}
		
		Vector3 target3 = current.Value.toVector3();
		Vector2 target = new Vector2(target3.x,target3.z);
		Vector2 myPos = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		return (target - myPos).normalized * this.getLegs().equilibrium;
	}
}