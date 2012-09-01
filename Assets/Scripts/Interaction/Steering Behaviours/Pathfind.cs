using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Pathfind steering behaviour */
/* example:
 * Pathfind b = new Pathfind();
 * b.Init(this.legs,this.grid);
 * 
 * b.setTarget(homebase) */
public class Pathfind : TargetableSteeringBehaviour {
	private Grid grid;
	private LinkedList<GridSquare> path;
	private LinkedListNode<GridSquare> current;
	public void Init(Legs legs,Grid grid) {
		base.Init(legs);
		this.grid = grid;
	}
	
	public override void onTarget ()
	{
		Vector2 target2 = getTarget ();
		Vector3 target = new Vector3(target2.x,0,target2.y);
		
		path = grid.findPath (this.getLegs().transform.position,target);
		
		if (path != null)
			current = path.First;
		else
			current = null;
	}
	
	public override Vector2 _getDesiredVelocity() {
		
		if (current == null)
		{
			this.setInternalWeight(0.0f);
			return Vector2.zero;
		}
		
		if (grid.gridSquareFromVector3(this.getLegs ().transform.position) == current.Value) {
			if (current.Next == null) {
				current = null;
				this.setInternalWeight(0.0f);
				return Vector2.zero;
			}
			current = current.Next;
		}
		
		Vector3 target3 = current.Value.toVector3();
		Vector2 target = new Vector2(target3.x,target3.z);
		Vector2 myPos = new Vector2(this.getLegs().transform.position.x,this.getLegs().transform.position.z);
		return (target - myPos).normalized * this.getLegs().equilibrium;
	}
}
