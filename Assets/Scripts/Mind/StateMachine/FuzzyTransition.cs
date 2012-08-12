using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuzzyTransition : State {
	public float fuzzyChance;

	public ExplicitStateReference optionOneRef;
	public ExplicitStateReference optionTwoRef;
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
}
