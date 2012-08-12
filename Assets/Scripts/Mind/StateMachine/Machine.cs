using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The state machine is itself a state, to enable easy nested state machines!
public class Machine<T> : MonoBehaviour {
	
	// Will implement an override queue of some kind, probably using Andrew's PriorityQueue<T> class,
	// as soon as I can work out an elegant way of integrating them with the normal state transitions.
    public T owner;

	public State<T> startingState;
	private State<T> currentState;	
	private State<T> previousState;
	
	//public List<State<T>> controlledStates;

    // Code for managing the passing of specific information to triggers.
    public delegate System.IComparable ObservedVariable();

    //public abstract ObservedVariable[] GetExposedVariables();
    //public List<TriggerManager> triggers;

	public void Awake() {
        /*
        foreach (TriggerManager trig in triggers)
        {
            trig.owner = this;
            trig.BuildTrigger();
        }
         */
		currentState = null;
        startingState = null;
        previousState = null;
        /*
		if(currentState == null)
		{
			Debug.LogError ("Initial State not set! Disabling.");
			gameObject.active = false;
		}
         */
	}

    public void Configure(T owner, State<T> startingState)
    {
        this.owner = owner;
        ChangeState(startingState);
    }

    public void Update()
    {
        if (currentState != null) currentState.Run(owner);
    }

    public void ChangeState(State<T> NewState)
    {
        previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit(owner);
        }
        currentState = NewState;
        if (currentState != null)
        {
            currentState.Enter(owner);
        }
    }

    public void RevertToPreviousState()
    {
        if (previousState != null)
            ChangeState(previousState);
    }

	public ObservedVariable[] GetExposedVariables ()
	{
		return new ObservedVariable[0];
	}
	
    public State<T> getCurrentState()
    {
        return this.currentState;
    }

    public void setCurrentState(State<T> state)
    {
        this.currentState = state;
    }



    /*
    public override IEnumerator Enter(T owner) {
        yield return StartCoroutine(currentState.Enter(owner));
    }
    public override IEnumerator Exit(T owner) {
        yield return StartCoroutine(currentState.Exit(this));
    }
	
    public override IEnumerator Run(Machine owner, Brain controller) {
        yield return StartCoroutine(currentState.Run(this, controller));
		
        if(nextState != null) {
            yield return currentState.Exit(this);
            currentState = nextState;
            nextState = null;
            yield return currentState.Enter(this);
        }
    }
     
    // Schedules a state transition at the next available time
	public void RequestStateTransition(State goToThis) {
		nextState = goToThis;
	}
    */
}
