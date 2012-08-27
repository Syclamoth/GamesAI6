using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The state machine is itself a state, to enable easy nested state machines!
public class Machine : State {
	
	
	// Will implement an override queue of some kind, probably using Andrew's PriorityQueue<T> class,
	// as soon as I can work out an elegant way of integrating them with the normal state transitions.
	
	public State startingState;
	
	private State currentState;
	
	private State nextState = null;
	
	public List<State> controlledStates;
	public List<TriggerManager> controlledTriggers;
	
	private bool triggersBuilt = false;
	
	void Awake() {
		currentState = startingState;
		if(currentState == null)
		{
			Debug.LogError ("Initial State not set! Disabling.");
			gameObject.active = false;
		}
	}
	
	public State GetCurrentState() {
		return currentState;
	}
	
	public override IEnumerator Enter(Machine owner, Brain controller) {
		yield return StartCoroutine(currentState.Enter(this, controller));
	}
	public override IEnumerator Exit() {
		yield return StartCoroutine(currentState.Exit());
	}
	
	public override IEnumerator Run(Brain controller) {
		if(!triggersBuilt) {
			foreach(TriggerManager manager in controlledTriggers) {
				manager.BuildTrigger(controller);
			}
			triggersBuilt = true;
		}
		yield return StartCoroutine(currentState.Run(controller));
		
		foreach(TriggerManager manager in currentState.GetTriggers()) {
			if(manager.ShouldTrigger()) {
				nextState = manager.target;
			}
		}
		if(nextState != null) {
			yield return StartCoroutine(currentState.Exit());
			currentState = nextState;
			nextState = null;
			yield return StartCoroutine(currentState.Enter(this, controller));
		}
	}
	
	public override ObservedVariable[] GetExposedVariables ()
	{
		return new ObservedVariable[0];
	}
	
	// Schedules a state transition at the next available time
	public void RequestStateTransition(State goToThis) {
		nextState = goToThis;
	}
	
	override public List<LinkedStateReference> GetStateTransitions() {
		List<LinkedStateReference> retV = new List<LinkedStateReference>();
		return retV;
	}
	override public string GetNiceName() {
		return "State Machine";
	}
	override public void DrawInspector() {
		
	}
	override public int DrawObservableSelector(int currentlySelected) {
		//string[] gridLabels = new string[] {};
		//return GUILayout.SelectionGrid(currentlySelected, gridLabels,1);
		return 0;
	}
}
