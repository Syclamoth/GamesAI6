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
		Vector3 raycastDirection = (getLegs().getVelocity().normalized * 1.1f).ToWorldCoords() + Random.insideUnitCircle.ToWorldCoords();
		if(boxes.Raycast(new Ray(getLegs().myTrans.position, raycastDirection), out curDistance, out curNormal)) {
			
			float maxDistance = getLegs().getVelocity().magnitude * predictionTime;
			Debug.DrawRay(getLegs().myTrans.position, raycastDirection * maxDistance, Color.cyan);
			//Debug.Log (maxDistance);
			if(curDistance > maxDistance) {
				return getLegs().getVelocity();
			} else {
				//Debug.Log(curNormal);
				//Debug.DrawRay(new Ray(getLegs().getPosition().ToWorldCoords(), raycastDirection).GetPoint(curDistance), curNormal, Color.red);
				setInternalWeight(100);
				return new Vector2(curNormal.normalized.x, curNormal.normalized.z) * getLegs().getVelocity().magnitude * 10;
			}
		}
		
		return getLegs().getVelocity();
	}
}
