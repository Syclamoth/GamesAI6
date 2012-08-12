using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Using the same 'interface that is not an interface' workaround that we used for senses.
public abstract class State<T> : MonoBehaviour {
	
	/* 
	 * Enter and exit are to be used with Unity's coroutine scheduler,
	 * so that they have the option of including behaviour modelled over several frames.
	 */
	public abstract IEnumerator Enter(T owner);
	public abstract IEnumerator Exit(T owner);
	
	// Run will be called from within an update loop, but can be used to delay execution.
	public abstract IEnumerator Run(T owner);     
}
