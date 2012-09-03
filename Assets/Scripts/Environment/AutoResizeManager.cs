using UnityEngine;
using System.Collections;

public class AutoResizeManager : MonoBehaviour {

	public Light lamp;
	float baseLampRadius;
	
	private float currentScale;
	
	void Awake() {
		InitialiseScales();
		currentScale = transform.localScale.magnitude;
		UpdateAllScales();
	}
	
	void InitialiseScales() {
		baseLampRadius = lamp.range;
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Mathf.Abs(currentScale - transform.localScale.magnitude) > 0.2f) {
			// Update all scales
			UpdateAllScales();
			currentScale = transform.localScale.magnitude;
		}
	}
	
	void UpdateAllScales() {
		lamp.range = baseLampRadius * currentScale;
	}
}
