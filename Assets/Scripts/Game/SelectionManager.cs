using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

	public SensableObjects allObjects;
	
	GameObject selected;
	
	void Update() {
		if(Input.GetMouseButtonDown(1)) {
			Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
			float hitPoint;
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(floorPlane.Raycast(mouseRay, out hitPoint)) {
				ChangeSelection(mouseRay.GetPoint(hitPoint));
			}
		}
	}
	
	void ChangeSelection(Vector3 clickPos) {
		if(selected != null) {
			selected.SendMessage("EnableGUI", false, SendMessageOptions.DontRequireReceiver);
			selected = null;
		}
		
		List<SensableObject> clickResult = allObjects.GetObjectsInRadius(clickPos, 0.5f);
		
		if(clickResult.Count > 0) {
			selected = clickResult[0].obj;
			selected.SendMessage("EnableGUI", true, SendMessageOptions.DontRequireReceiver);
		}
	}
}
