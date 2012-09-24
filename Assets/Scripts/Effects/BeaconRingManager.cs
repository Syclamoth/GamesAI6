using UnityEngine;
using System.Collections;

public class BeaconRingManager : MonoBehaviour {
	
	
	void RunExplosion (float endRadius) {
		StartCoroutine(ExpandRing (endRadius / 2));
	}
	
	IEnumerator ExpandRing (float maxSize) {
		float curTime = 0;
		Color startColour;
		
		transform.localScale = Vector3.zero;
		startColour = renderer.material.GetColor("_TintColor");
		AnimationCurve expansionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		expansionCurve.keys[0].outTangent = 90;
		while(curTime < 1) {
			transform.localScale = Vector3.one * maxSize * expansionCurve.Evaluate(curTime);
			renderer.material.SetColor("_TintColor", Color.Lerp(startColour, Color.black, curTime));
			curTime += Time.deltaTime;
			yield return null;
		}
		Destroy(gameObject);
	}
	
	
}
