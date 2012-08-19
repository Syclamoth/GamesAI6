using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxManager : MonoBehaviour {
	
	private List<Bounds> allBoxes = new List<Bounds>();
	
	public void AddBox(Bounds addThis) {
		allBoxes.Add(addThis);
	}
	
	public bool Raycast(Ray input, out float distance, out Vector3 normal) {
		distance = float.MaxValue;
		normal = Vector3.zero;
		bool retV = false;
		foreach(Bounds box in allBoxes) {
			float curDistance;
			if(box.IntersectRay(input, out curDistance)) {
				normal = distance < curDistance ? (input.GetPoint(curDistance) - box.center) : normal;
				distance = distance < curDistance ? distance : curDistance;
				retV = true;
			}
		}
		return retV;
	}
}
