using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetupSheepSteering : State {
	public ExplicitStateReference done = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;
	private Cohesion cohesion;
	private Alignment alignment;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLegs = myBrain.legs;
		cohesion = new Cohesion(controller.allObjects);
		cohesion.setWeight (0.3f);
		cohesion.Init (myLegs);
		alignment = new Alignment(controller.allObjects);
		alignment.Init (myLegs);
		alignment.setWeight(0.2f);
		myLegs.addSteeringBehaviour(cohesion);
		myLegs.addSteeringBehaviour(alignment);
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
		mainMachine.RequestStateTransition(done.GetTarget());
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
        retV.Add(new LinkedStateReference(done, "Finished"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Setup Sheep";
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
