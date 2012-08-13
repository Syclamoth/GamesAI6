using UnityEngine;
using System.Collections;

public class TargetableSteeringBehaviour : SteeringBehaviour {
	private Vector2 target = default(Vector2);
	private GameObject targetObject = null;
	public void setTarget(Vector2 target) {
		this.target = target;
	}
	public void setTarget(Vector3 target) {
		this.target = new Vector2(target.x,target.z);
	}
	public void setTarget(GameObject target) {
		this.targetObject = target;
	}
	public Vector2 getTarget() {
		if (this.targetObject != null)
			this.setTarget(targetObject.transform.position);
		return this.target;
	}
}
