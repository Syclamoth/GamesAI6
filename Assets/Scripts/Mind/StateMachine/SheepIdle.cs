using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepIdle : State {
	
	public ExplicitStateReference alarm = new ExplicitStateReference(null);
	
	public SheepCharacteristics stats;
	
	Machine mainMachine;
	
	public override IEnumerator Enter(Machine owner, Brain controller) {
		mainMachine = owner;
		yield return null;
	}
	public override IEnumerator Exit() {
		yield return null;
	}
	public override IEnumerator Run(Brain controller) {
		
		foreach(SensedObject obj in controller.senses.GetSensedObjects()) {
			if(obj.getAgentType() == AgentClassification.Wolf) {
				controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") + 10);
			}
		}
		
		if((float)controller.memory.GetValue("Panic") >= 85) {
			mainMachine.RequestStateTransition(alarm.GetTarget());
		}
		yield return null;
	}
	public override ObservedVariable[] GetExposedVariables ()
	{
		return new ObservedVariable[] {
		};
	}
	
	
	override public List<LinkedStateReference> GetStateTransitions() {
		List<LinkedStateReference> retV = new List<LinkedStateReference>();
		retV.Add(new LinkedStateReference(alarm, "Alarm"));
		return retV;
	}
	
	
	//State Machine editor
	override public string GetNiceName() {
		return "Sheep Idle";
	}
	override public void DrawInspector() {
		//stats = (SheepCharacteristics)EditorGUILayout.ObjectField(stats, typeof(SheepCharacteristics), true);
	}
	override public int DrawObservableSelector(int currentlySelected) {
		string[] gridLabels = new string[] {
		};
		return GUILayout.SelectionGrid(currentlySelected, gridLabels,1);
	}
}
