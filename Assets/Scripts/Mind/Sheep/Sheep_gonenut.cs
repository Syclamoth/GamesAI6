using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_gonenut : State {

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        //see wolf, panic!
        if (controller.senses.isContainAgent(AgentClassification.Wolf))
        {
            if (controller.memory.GetValue<float>("Panic") < 100) //maximum panic is 100
            {
                //update panic level of the sheep when it sees a Wolf, according to his courage level which is ranged from 0.0 to 1.0
                controller.memory.SetValue("Panic", (float)controller.memory.GetValue("Panic") + 1.1 * controller.memory.GetValue<float>("courageLevel"));
            }
        }
        //can't see any wolf around
        else
        {
            controller.memory.SetValue("Panic", (float)controller.memory.GetValue("Panic") - 0.4 * controller.memory.GetValue<float>("courageLevel"));
        }

        //do the gonenut. It can stand at the ground or just running randomly, wildly. Need help with whatever are coded inside sheeplegs. 
        //First, speed must be increased, second, it should run randomly or just stand at a spot, or just run slowly slowly.

        // if panic level larger than 7, change to gonenuts state.
        if ((float)controller.memory.GetValue("Panic") < 50)
        {
            mainMachine.RequestStateTransition(alarm.GetTarget());
        }
        // if can't see wolf and panic level has decreased, change to roaming state
        if ((float)controller.memory.GetValue("Panic") < 7)
        {
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
