using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_beingeaten : State
{

    public ExplicitStateReference nuts = new ExplicitStateReference(null);
    
    Machine mainMachine;
    Brain myBrain;

    private float decayFollowRate = 0.5f;
    private float increaseFollowRate = 3f;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 4f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;

        //speed is zero
        myBrain.legs.maxSpeed = 0f;
        controller.memory.SetValue("Panic", 50f);

        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheperd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsSheperd = true;
            }
        }

        if (thereIsSheperd)
        {
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
        }
        else
        {
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * (increasePanicRate) * controller.memory.GetValue<float>("cowardLevel")));

            if (controller.memory.GetValue<float>("Panic") > 55f)
            {
                controller.memory.SetValue("Panic", 55f);
            }
        }

        //back to gonenut state
        if (controller.memory.GetValue<float>("Panic") < 40f)
        {
            mainMachine.RequestStateTransition(nuts.GetTarget());
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
        retV.Add(new LinkedStateReference(nuts, "Insane"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Sheep Being Eaten";
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
