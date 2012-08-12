using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuzzyTransition : State {
	public float fuzzyChance;

	public ExplicitStateReference optionOneRef = new ExplicitStateReference(null);
	public ExplicitStateReference optionTwoRef = new ExplicitStateReference(null);
	public override IEnumerator Enter(Machine owner) {
		owner.RequestStateTransition(fuzzyChance > Random.value ? optionOneRef.GetTarget() : optionTwoRef.GetTarget());
		yield return null;
	}
	public override IEnumerator Exit() {
		yield return null;
	}
	public override IEnumerator Run(Brain controller) {
		yield return null;
	}
	public override ObservedVariable[] GetExposedVariables ()
	{
		return new ObservedVariable[0];
	}
	
	override public List<LinkedStateReference> GetStateTransitions() {
		List<LinkedStateReference> retV = new List<LinkedStateReference>();
		
		retV.Add(new LinkedStateReference(optionOneRef, "Option 1"));
		retV.Add(new LinkedStateReference(optionTwoRef, "Option 2"));
		return retV;
	}
	
	override public string GetNiceName() {
		return "Fuzzy Transition";
	}
	
	override public void DrawInspector() {
		GUILayout.Label("1 <----------> 2");
		fuzzyChance = GUILayout.HorizontalSlider(fuzzyChance * 100, 0, 100) / 100;
	}
	
	override public int DrawObservableSelector(int currentlySelected) {
		//string[] gridLabels = new string[] {};
		//return GUILayout.SelectionGrid(currentlySelected, gridLabels,1);
		return 0;
	}
}
