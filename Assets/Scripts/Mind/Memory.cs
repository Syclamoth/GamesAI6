using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryEntry {
	private string key;
	private object value;
	
	public string GetKey() {
		return key;
	}
	
	public object GetValue() {
		return value;
	}
	
	public MemoryEntry(string key, object value) {
		this.key = key;
		this.value = value;
	}
}

public class Memory {
	private Dictionary<string, object> objectRegistry = new Dictionary<string, object>();
	
	public object GetValue(string key) {
		if(objectRegistry.ContainsKey(key)) {
			return objectRegistry[key];
		} else {
			return 0;
		}
	}
	

	/*
	 * A generic version of GetValue. Reduces the need for messy casts in other code by performing type checking internally.
	 * Will return a default value (either 0 or null) if there is no such key, or the returned datatype is incompatible.
	 */
	public T GetValue<T>(string key) {
		if(!objectRegistry.ContainsKey(key)) {
			return default(T);
		}
		T retV;
		try{
			retV = (T)objectRegistry[key];
		} catch (System.InvalidCastException e) {
			Debug.LogError (e.StackTrace);
			retV = default(T);
		}
		return retV;
	}
	
	public void SetValue(string key, object value) {
		if(objectRegistry.ContainsKey(key)) {
			objectRegistry[key] = value;
		} else {
			objectRegistry.Add(key, value);
		}
	}
	
	public void SetValue(MemoryEntry input) {
		SetValue (input.GetKey(), input.GetValue());
	}
	
}
