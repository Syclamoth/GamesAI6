using UnityEngine;
using System.Collections;

public class FuzzyTransition : State {
	public float fuzzyChance;
	
	public State optionOne;
	public State optionTwo;
	
	public override IEnumerator Enter(Machine owner) {
		owner.RequestStateTransition(fuzzyChance > Random.value ? optionOne : optionTwo);
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
}
