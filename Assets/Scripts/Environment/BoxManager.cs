using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxManager : MonoBehaviour {
	
	public List<BoxCollider> startingBoxes;
	
	private List<Bounds> allBoxes = new List<Bounds>();
	
	void Awake() {
		foreach(BoxCollider curBox in startingBoxes) {
			AddBox(curBox.bounds);
		}
	}
	
	public void AddBox(Bounds addThis) {
		allBoxes.Add(addThis);
	}
	
	public bool Raycast(Ray input, out float distance, out Vector3 normal) {
		distance = float.MaxValue;
		normal = Vector3.zero;
		bool retV = false;
		//Debug.DrawRay(input.origin, input.direction, Color.blue);
		foreach(Bounds box in allBoxes) {
			float curDistance;
			if(box.Contains(input.origin)) {
				curDistance = 0; //Vector3.Distance(box.center, input.origin);
				//if(curDistance < distance) {
					distance = curDistance;
				//}
				normal = (input.origin - box.center).normalized;
				break;
			}
			if(box.IntersectRay(input, out curDistance)) {
				if(curDistance < distance) {
					normal = Vector3.zero;
					Vector3 impactPoint = input.GetPoint(curDistance -0.1f);
					if(impactPoint.x > box.max.x) {
						normal += Vector3.right;
					} else if(impactPoint.x < box.min.x) {
						normal -= Vector3.right;
					}
					if(impactPoint.z > box.max.z) {
						normal += Vector3.forward;
					} else if(impactPoint.z < box.min.z) {
						normal -= Vector3.forward;
					}
					distance = curDistance;
				}
				retV = true;
				//Debug.DrawRay(input.GetPoint(curDistance), normal, Color.red);
			}
		}
		return retV;
	}
}
