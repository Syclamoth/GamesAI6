using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Memory {
	private Dictionary<string, object> objectRegistry = new Dictionary<string, object>();
	
	public object GetValue(string key) {
		if(objectRegistry.ContainsKey(key)) {
			return objectRegistry[key];
		} else {
			return 0;
		}
	}
	
	public void SetValue(string key, object value) {
		if(objectRegistry.ContainsKey(key)) {
			objectRegistry[key] = value;
		} else {
			objectRegistry.Add(key, value);
		}
	}
	
}
