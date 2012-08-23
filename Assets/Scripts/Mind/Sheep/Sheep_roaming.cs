using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_roaming : State {
    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;
	
	private bool firstActivation = false;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		if(firstActivation)
		{
        	//set courageLevel for sheep
        	myBrain.memory.SetValue("courageLevel", Random.value);
			firstActivation = false;
		}
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
		// ALL THESE STATES ARE BROKEN! I'm disabling them until tomorrow, when we can sort this out.
		//yield break;
		
        if (controller.senses.isContainAgent(AgentClassification.Wolf))
        {
            //update panic level of the sheep when it sees a Wolf, according to his courage level which is ranged from 0.0 to 1.0
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + Time.deltaTime * controller.memory.GetValue<float>("courageLevel"));
        }
        
        //do the roaming, need help with whatever are coded inside sheeplegs

        // if panic level larger than 7, change to running state.
        if (controller.memory.GetValue<float>("Panic") >= 7)
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
        return "Sheep Roaming";
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
