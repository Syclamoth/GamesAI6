using UnityEngine;
using System.Collections;

public class SensedObject {

	private GameObject sensedObject;
    public SensedObject(GameObject obj)
    {
        sensedObject = obj;
    }
    public GameObject getObject()
    {
        return sensedObject;
    }
}
