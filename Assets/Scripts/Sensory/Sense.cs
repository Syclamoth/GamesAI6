using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Sense : MonoBehaviour
{
	public abstract List<SensedObject> SensedObjects ();
}
