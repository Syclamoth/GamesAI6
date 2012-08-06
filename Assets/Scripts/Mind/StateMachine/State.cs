using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	
	public List<TriggerManager> triggers;
	
	public void Awake() {
		foreach(TriggerManager trig in triggers) {
			trig.owner = this;
			trig.BuildTrigger();
		}
	}
}
