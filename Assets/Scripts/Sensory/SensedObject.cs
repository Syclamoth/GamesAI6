using UnityEngine;
using System.Collections;

public class SensedObject {

	private GameObject sensedObject;
	private AgentClassification type;
    

    public SensedObject(GameObject obj)
    {
        sensedObject = obj;
		type = AgentClassification.Unknown;
    }
	public SensedObject(GameObject obj, AgentClassification newType)
    {
        sensedObject = obj;
		type = newType;
    }
    public GameObject getObject()
    {
        return sensedObject;
    }
	public AgentClassification getAgentType() {
		return type;
	}
    public Memory getMemory()
    {
        Brain thisObjBrain = (Brain)sensedObject.GetComponent("Brain");
        
        return thisObjBrain.memory;
    }
}
