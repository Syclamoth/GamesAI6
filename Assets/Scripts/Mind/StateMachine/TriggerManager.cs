using UnityEngine;
using System.Collections;


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
	
	public string memoryKey;
	
	public TriggerMode mode;
	
	private Brain watchedBrain = null;
	
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
	
	public void BuildTrigger(Brain cortex) {
		if(owner != null)
		{
			owner.AddTrigger(this);
		}
		if(watched != null)
		{
			observed = watched.GetExposedVariables()[observedIndex];
		} else {
			watchedBrain = cortex;
			observed = GetMemoryData;
		}
	}
	
	System.IComparable GetMemoryData() {
		System.IComparable retV = 0;
		try {
			retV = (System.IComparable)watchedBrain.memory.GetValue(memoryKey);
		} catch (System.InvalidCastException e) {
			Debug.LogWarning("The data stored in " + memoryKey + " is not compatible with triggers! Make sure you are using the correct key.");
			Debug.LogError (e);
			retV = 0;
		}
		return retV;
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
		case ObservedType.other:
			targetValue = floatTarget;
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
