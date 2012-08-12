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
	
	new void Awake() {
		base.Awake();
		currentState = startingState;
		if(currentState == null)
		{
			Debug.LogError ("Initial State not set! Disabling.");
			gameObject.active = false;
		}
	}
	
	public override IEnumerator Enter(Machine owner) {
		yield return StartCoroutine(currentState.Enter(this));
	}
	public override IEnumerator Exit() {
		yield return StartCoroutine(currentState.Exit());
	}
	
	public override IEnumerator Run(Brain controller) {
		yield return StartCoroutine(currentState.Run(controller));
		
		if(nextState != null) {
			yield return currentState.Exit();
			currentState = nextState;
			nextState = null;
			yield return currentState.Enter(this);
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
}
