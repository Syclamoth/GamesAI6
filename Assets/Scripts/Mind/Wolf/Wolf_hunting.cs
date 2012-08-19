using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_hunting : State
{

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    private float time;
    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;

        time = 0;
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        if (controller.senses.isContainAgent(AgentClassification.Sheep))
        {
            //if the wolf can still see his target, chase it
            controller.memory.GetValue<SensedObject>("hasCommand");   

        }

        //if the wolf can't see his target anymore, chase it for 10sec to gain vision, if it still can't find the sheep, change to roaming state.
        else
        {
            time += Time.deltaTime;
            if (time > 10)
            {
                //delete its target
                controller.memory.SetValue("hasCommand", null);   

                //change to roaming state
                mainMachine.RequestStateTransition(alarm.GetTarget());
            }
            else
            {
                //keep chasing
                controller.memory.GetValue<SensedObject>("hasCommand");   
            }
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
        retV.Add(new LinkedStateReference(alarm, "Lost Target"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Hunting";
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
