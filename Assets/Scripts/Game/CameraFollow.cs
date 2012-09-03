using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform[] followThese;
	
	private Transform _myTrans;
	
	private Transform myTrans {
		get {
			if(_myTrans == null) {
				_myTrans = transform;
			}
			return _myTrans;
		}
	}
	
	void LateUpdate () {
		Vector3 averagePosition = Vector3.zero;
		if(followThese.Length > 0) {
			foreach(Transform trans in followThese) {
				averagePosition += trans.position;
			}
			averagePosition /= followThese.Length;
		}
		myTrans.position = Vector3.Lerp(myTrans.position, averagePosition, Time.deltaTime * 5);
	}
}
