using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Idle : State {

	public ExplicitStateReference alarm = new ExplicitStateReference(null);
	public override IEnumerator Enter(Machine owner) {
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
		return new ObservedVariable[] {
			GetPanicLevel
		};
	}
	
	override public List<LinkedStateReference> GetStateTransitions() {
		List<LinkedStateReference> retV = new List<LinkedStateReference>();
		
		retV.Add(new LinkedStateReference(alarm, "Alarm"));
		return retV;
	}
	override public string GetNiceName() {
		return "Idle";
	}
	override public void DrawInspector() {
		
	}
	override public int DrawObservableSelector(int currentlySelected) {
		string[] gridLabels = new string[] {
			"Panic Level"
		};
		return GUILayout.SelectionGrid(currentlySelected, gridLabels,1);
	}
	
	public System.IComparable GetPanicLevel() {
		return 0;
	}
}