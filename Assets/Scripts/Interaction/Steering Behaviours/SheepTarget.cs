using UnityEngine;
using System.Collections;

public class SheepTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		
		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
		    Vector3 newPos = hit.point;
			newPos.y = 0;
			transform.position = newPos;
		}
		
	}
}
