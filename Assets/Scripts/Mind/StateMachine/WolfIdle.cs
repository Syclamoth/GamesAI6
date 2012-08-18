using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfIdle : State {
	
	public ExplicitStateReference attack = new ExplicitStateReference(null);
	
	Machine mainMachine;
	
	private RandomWalk wander = new RandomWalk(0.6f, 1, 60);
	
	private Brain myBrain;
	
	public override IEnumerator Enter(Machine owner, Brain controller) {
		mainMachine = owner;
		myBrain = controller;
		
		myBrain.legs.addSteeringBehaviour(wander);
		yield return null;
	}
	public override IEnumerator Exit() {
		myBrain.legs.removeSteeringBehaviour(wander);
		yield return null;
	}
	public override IEnumerator Run(Brain controller) {
		
		foreach(SensedObject obj in controller.senses.GetSensedObjects()) {
			if(obj.getAgentType() == AgentClassification.Sheep) {
				controller.memory.SetValue("SeenTarget", obj.getObject());
				
			}
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
		retV.Add(new LinkedStateReference(attack, "Attack"));
		return retV;
	}
	
	
	//State Machine editor
	override public string GetNiceName() {
		return "Wolf Wander";
	}
	override public void DrawInspector() {
	}
	override public int DrawObservableSelector(int currentlySelected) {
		string[] gridLabels = new string[] {
		};
		return GUILayout.SelectionGrid(currentlySelected, gridLabels,1);
	}
}
