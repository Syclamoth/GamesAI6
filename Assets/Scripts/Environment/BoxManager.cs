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
		Debug.DrawRay(input.origin, input.direction, Color.blue);
		foreach(Bounds box in allBoxes) {
			float curDistance;
			if(box.IntersectRay(input, out curDistance)) {
				if(curDistance < distance) {
					normal = (input.GetPoint(curDistance) - box.center).normalized;
					distance = curDistance;
				}
				retV = true;
				Debug.DrawRay(input.GetPoint(curDistance), normal, Color.red);
			}
		}
		return retV;
	}
}
