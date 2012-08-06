using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SensoryCortex : MonoBehaviour {

	public Sense[] senses;

    public HashSet<SensedObject> GetSensedObjects()
    {
        HashSet<SensedObject> seenObjects = new HashSet<SensedObject>();
        for (int i = 0; i < senses.Length; i++)
        {
            seenObjects.UnionWith(senses[i].SensedObjects());
        }
        return seenObjects;
    }
}
