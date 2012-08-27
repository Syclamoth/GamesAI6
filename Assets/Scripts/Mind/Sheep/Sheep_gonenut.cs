using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_gonenut : State {

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 4f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;

        //set speed to minimum
        myBrain.legs.maxSpeed = 0.1f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheperd = false;
        bool thereIsWolf = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                thereIsWolf = true;
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsSheperd = true;
            }
        }

        if (thereIsWolf)
        {
            //the less cowardLevel is, the less Panic increases. However, in this state, the sheep has its panic level increased by 1.25
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * (increasePanicRate * 1.25f) * controller.memory.GetValue<float>("cowardLevel")));

            if (controller.memory.GetValue<float>("Panic") > 90f)
            {
                controller.memory.SetValue("Panic", 90f);
            }
        }
        else
        {
            //if there is no wolf around and it can see the Sheperd, the recover rate from Panic is doubled.
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * 2 * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            }
            else
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            }

            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
            }
        }

        /*
        if (controller.senses.isContainAgent(AgentClassification.Wolf))
        {
            //the less cowardLevel is, the less Panic increases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));

            if (controller.memory.GetValue<float>("Panic") > 90f)
            {
                controller.memory.SetValue("Panic", 90f);
            }
        }
        else
        {
            //if there is no wolf around and it can see the Sheperd, the recover rate from Panic is doubled.
            if (controller.senses.isContainAgent(AgentClassification.Shepherd))
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * 2 * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            }
            else
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            }

            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
            }
        }*/

        // if can't see wolf and panic level has decreased, change to roaming state
        if (controller.memory.GetValue<float>("Panic") < 7f)
        {
            Debug.Log("Wolf's gone and I'm calm now!");
            mainMachine.RequestStateTransition(alarm.GetTarget());
        }
        yield return null;
    }
    public override ObservedVariable[] GetExposedVariables()
    {
        return new ObservedVariable[] {
		};
    }


    override public List<LinkedStateReference> GetStateTransitions()
    {
        List<LinkedStateReference> retV = new List<LinkedStateReference>();
        retV.Add(new LinkedStateReference(alarm, "Alarm"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Sheep Gonenut";
    }
    override public void DrawInspector()
    {
        //stats = (SheepCharacteristics)EditorGUILayout.ObjectField(stats, typeof(SheepCharacteristics), true);
    }
    override public int DrawObservableSelector(int currentlySelected)
    {
        string[] gridLabels = new string[] {
		};
        return GUILayout.SelectionGrid(currentlySelected, gridLabels, 1);
    }
}
