using UnityEngine;
using System.Collections;

public class MousePosition : MonoBehaviour {
	
	private Transform _myTrans;
	
	private Transform myTrans {
		get {
			if(_myTrans == null) {
				_myTrans = transform;
			}
			return _myTrans;
		}
	}
	
	private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
	
	void Update () {
		float distance;
		Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(groundPlane.Raycast(screenRay, out distance)) {
			transform.position = screenRay.GetPoint(distance);
		}
	}
}
