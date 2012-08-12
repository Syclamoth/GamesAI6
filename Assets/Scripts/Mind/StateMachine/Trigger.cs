using UnityEngine;
using System.Collections;
/*
public class Trigger<obsType> : Object where obsType : System.IComparable, System.IEquatable<obsType> {

    private State<T>.ObservedVariable observed;
	private obsType targetValue;
	private State<T> goToState;
	private TriggerMode mode;

    public Trigger(State<T> targetState, State<T>.ObservedVariable obsValue, obsType targetV, TriggerMode trMode)
    {
		targetValue = targetV;
		observed = obsValue;
		if(observed().GetType() != typeof(obsType)) {
			Debug.LogError ("Incorrect type specified in constructor!");
		}
		goToState = targetState;
		mode = trMode;
	}

    public State<T> GetTargetState()
    {
		return goToState;
	}
	
	public bool ShouldTrigger() {
		switch (mode) {
		case TriggerMode.Equal:
			return targetValue.Equals((obsType)observed());
		case TriggerMode.GEqual:
			return targetValue.CompareTo((obsType)observed()) >= 0;
		case TriggerMode.LEqual:
			return targetValue.CompareTo((obsType)observed()) <= 0;
		case TriggerMode.Greater:
			return targetValue.CompareTo((obsType)observed()) > 0;
		case TriggerMode.Lesser:
			return targetValue.CompareTo((obsType)observed()) < 0;
		}
		return false;
	}
	
}

public enum TriggerMode {
	Greater,
	Lesser,
	Equal,
	GEqual,
	LEqual
}
*/