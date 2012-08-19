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

    //get every sheep inside the wolf's radius
    public List<SensedObject> GetSensedSheep()
    {
        List<SensedObject> seenSheep = new List<SensedObject>();

        foreach (SensedObject obj in this.GetSensedObjects())
        {
            //if (obj.getAgentType() == AgentClassification.Wolf)
            if (obj.getAgentType() == AgentClassification.Sheep)
            {
                seenSheep.Add((SensedObject)obj);
            }
        }

        return seenSheep;
    }

    //get every sheep inside the wolf's radius
    public List<SensedObject> GetSensedWolf()
    {
        List<SensedObject> seenWolf = new List<SensedObject>();

        foreach (SensedObject obj in this.GetSensedObjects())
        {
            //if (obj.getAgentType() == AgentClassification.Wolf)
            if (obj.getAgentType() == AgentClassification.Wolf)
            {
                seenWolf.Add((SensedObject)obj);
            }
        }

        return seenWolf;
    }

    //check if there are wolf, sheep or sheperd, unknown object in SensedObject array
    public bool isContainAgent(AgentClassification type)
    {
        bool flag = true;
        foreach (SensedObject obj in this.GetSensedObjects())
        {
            //if (obj.getAgentType() == AgentClassification.Wolf)
            if (obj.getAgentType() == type)
            {
                flag = false;
            }
        }
        
        return flag;
    }

    public float getHighestLeaderLevel(Brain controller)
    {
        float highest = 0;

        foreach (SensedObject obj in this.GetSensedObjects())
        {
            if (obj.getAgentType() == AgentClassification.Wolf)
            {
                if (highest <= controller.memory.GetValue<float>("leaderLevel"))
                {
                    highest = controller.memory.GetValue<float>("leaderLevel");
                }
            }
        }

        return highest;
    }
}
