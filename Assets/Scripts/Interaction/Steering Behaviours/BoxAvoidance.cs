using UnityEngine;
using System.Collections;

public class BoxAvoidance : SteeringBehaviour {


	BoxManager boxes;
	float predictionTime;
	
	public BoxAvoidance(BoxManager newBoxes, float distance) {
		boxes = newBoxes;
		predictionTime = distance;
	}
	
	public override Vector2 _getDesiredVelocity() {
		setInternalWeight(0);
		float curDistance;
		Vector3 curNormal;
		Vector3 raycastDirection = (Vector3)(getLegs().getVelocity().normalized * 2) + Random.insideUnitCircle.ToWorldCoords();
		if(boxes.Raycast(new Ray(getLegs().getPosition(), raycastDirection), out curDistance, out curNormal)) {
			float maxDistance = getLegs().getVelocity().magnitude * predictionTime;
			if(curDistance > maxDistance) {
				return getLegs().getVelocity();
			}
			setInternalWeight(Mathf.Lerp(10, 1, (maxDistance - curDistance) / maxDistance));
			return curNormal.normalized * getLegs().getVelocity().magnitude;
		}
		
		return getLegs().getVelocity();
	}
}
