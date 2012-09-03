using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_gonenut : State {

    public ExplicitStateReference roaming = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

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
            //if there is wolf around and it can see the Sheperd, the recover rate from Panic is doubled.
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * (increasePanicRate * 0.75f) * controller.memory.GetValue<float>("cowardLevel")));
            }
            else
            {
                //the less cowardLevel is, the less Panic increases. However, in this state, the sheep has its panic level increased by 1.5
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * (increasePanicRate * 1.5f) * controller.memory.GetValue<float>("cowardLevel")));
            }


            if (controller.memory.GetValue<float>("Panic") >= 50f)
            {
                controller.memory.SetValue("Panic", 50f);
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

        // if can't see wolf and panic level has decreased, change to roaming state
        if (controller.memory.GetValue<float>("Panic") < 7f)
        {
            Debug.Log("Wolf's gone and I'm calm now!");
            mainMachine.RequestStateTransition(roaming.GetTarget());
        }

        //if the sheep get caught
        if (controller.memory.GetValue<float>("Panic") >= 55f)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
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
        retV.Add(new LinkedStateReference(roaming, "Roaming"));
        retV.Add(new LinkedStateReference(eaten, "Being Eaten"));
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
