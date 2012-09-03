using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DebrisRandomiser : MonoBehaviour {

	public List<Transform> ignoreThese;
	void Start () {
		foreach(Transform curTrans in transform) {
			if(ignoreThese.Contains(curTrans)) {
				continue;
			}
			float maxDist = Mathf.Lerp(0.5f, 0, (curTrans.localPosition.y));
			curTrans.localPosition = new Vector3(Random.Range(-maxDist, maxDist), curTrans.localPosition.y, Random.Range(-maxDist, maxDist));
		}
	}
	
}
