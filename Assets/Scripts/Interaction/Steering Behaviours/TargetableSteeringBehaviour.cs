using UnityEngine;
using System.Collections;

public class TargetableSteeringBehaviour : SteeringBehaviour {
	private Vector2 target = default(Vector2);
	private GameObject targetObject = null;
	public void setTarget(Vector2 target) {
		this.target = target;
		this.onTarget ();
	}
	public void setTarget(Vector3 target) {
		this.target = new Vector2(target.x,target.z);
		this.onTarget ();
	}
	public void setTarget(GameObject target) {
		this.targetObject = target;
		this.onTarget ();
	}
	public Vector2 getTarget() {
		if (this.targetObject != null)
			this.target = new Vector2(targetObject.transform.position.x,targetObject.transform.position.z);
		return this.target;
	}
	public virtual void onTarget() {
	}
}
