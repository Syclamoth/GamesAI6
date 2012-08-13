using UnityEngine;
using System.Collections;

public class SheepTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		
		if(Input.GetMouseButtonDown(0)){
			//Failed mouse input
			Debug.Log ("OOGOSOSESFEFD");
			RaycastHit hit;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
		    Vector3 newPos = hit.point;
			Debug.Log(newPos);
			newPos.y = 0;
			transform.position = newPos;
		}
		
	}
}
