using UnityEngine;
using System.Collections;

// A serialisable type that can keep track of the connection between observed variables and actual triggers.
// At 'runtime', generates the type-safe trigger to be used.
[System.Serializable]
public class TriggerManager {
	public int observedIndex;
	public State owner;
	public State watched;
	public State target;
	
	public Vector2 inspectorCorner;
	
	public int intTarget;
	public float floatTarget;
	public bool boolTarget;
	
	public TriggerMode mode;
	
	private State.ObservedVariable observed;
	
	public ObservedType obsType {
		get {
			if(observed == null) {
				if(watched == null || watched.GetExposedVariables().Length <= observedIndex) {
					return ObservedType.other;
				}
				observed = watched.GetExposedVariables()[observedIndex];
			}
			if(observed().GetType() == typeof(int)) {
				return ObservedType.integer;
			}
			if(observed().GetType() == typeof(float)) {
				return ObservedType.floatingPoint;
			}
			if(observed().GetType() == typeof(bool)) {
				return ObservedType.boolean;
			}
			return ObservedType.other;
		}
	}
	
	public void BuildTrigger() {
		if(owner != null)
		{
			owner.AddTrigger(this);
		}
		if(watched != null)
		{
			observed = watched.GetExposedVariables()[observedIndex];
		}
	}
	
	public bool ShouldTrigger() {
		System.IComparable targetValue = 0;
		switch (obsType) {
		case ObservedType.integer:
			targetValue = intTarget;
			break;
		case ObservedType.floatingPoint:
			targetValue = floatTarget;
			break;
		case ObservedType.boolean:
			targetValue = boolTarget;
			break;
		}
		switch (mode) {
		case TriggerMode.Equal:
			return targetValue.Equals(observed());
		case TriggerMode.GEqual:
			return targetValue.CompareTo(observed()) >= 0;
		case TriggerMode.LEqual:
			return targetValue.CompareTo(observed()) <= 0;
		case TriggerMode.Greater:
			return targetValue.CompareTo(observed()) > 0;
		case TriggerMode.Lesser:
			return targetValue.CompareTo(observed()) < 0;
		}
		return false;
	}
}

// Ugly hardcoded type checking.
public enum ObservedType {
	integer,
	floatingPoint,
	boolean,
	other
}
