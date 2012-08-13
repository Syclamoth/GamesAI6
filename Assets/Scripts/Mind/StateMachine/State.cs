using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
[System.Serializable]
public class ExplicitStateReference {
	// Oh god this is disgusting.
	[UnityEngine.SerializeField]
	State target;
	public ExplicitStateReference(State newTarget) {
		target = newTarget;
	}
	public State GetTarget() {
		return target;
	}
	public void SetTarget(State newTarg) {
		target = newTarg;
	}
	
	public static explicit operator Reference<State>(ExplicitStateReference original){
		return new Reference<State>(original.target);
	}
}

public class Reference<T> {
	T target;
	public Reference(T newTarget) {
		target = newTarget;
	}
	public T GetTarget() {
		return target;
	}
	public void SetTarget(T newTarg) {
		target = newTarg;
	}
}

public class LinkedStateReference {
	string label;
	ExplicitStateReference reference;
	
	public LinkedStateReference(ExplicitStateReference newRef, string newLabel) {
		label = newLabel;
		reference = newRef;
	}
	
	public string GetLabel() {
		return label;
	}
	public State GetState() {
		return reference.GetTarget();
	}
	public void SetState(State newState) {
		reference.SetTarget(newState);
	}
}

// Using the same 'interface that is not an interface' workaround that we used for senses.
public abstract class State : MonoBehaviour {
	
	/* 
	 * Enter and exit are to be used with Unity's coroutine scheduler,
	 * so that they have the option of including behaviour modelled over several frames.
	 */
	public abstract IEnumerator Enter(Machine owner);
	public abstract IEnumerator Exit();
	
	// Run will be called from within an update loop, but can be used to delay execution.
	public abstract IEnumerator Run(Brain controller);
	
	
	// Code for managing the passing of specific information to triggers.
	public delegate System.IComparable ObservedVariable();
	public abstract ObservedVariable[] GetExposedVariables();
	
	public abstract List<LinkedStateReference> GetStateTransitions();
	
	
	private List<TriggerManager> triggers = new List<TriggerManager>();
	
	public void AddTrigger(TriggerManager newTrig)
	{
		triggers.Add(newTrig);
	}
	
	public List<TriggerManager> GetTriggers() {
		return triggers;
	}
	
	// Add inspector GUI code here- will show up inside the box in the editor.
	public abstract void DrawInspector();
	// A simple abstract selector, to allow a trigger to intelligently pick 
	public abstract int DrawObservableSelector(int currentlySelected);
	
	public abstract string GetNiceName();
	
	public Vector2 inspectorCorner;
}
