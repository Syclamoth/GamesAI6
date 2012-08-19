using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetupSteering : State {
	public ExplicitStateReference done = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;
	
	private Arrive seekBehaviour;
	private Separation separation;
	private Cohesion cohesion;
	private Alignment alignment;
	private RandomWalk random;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLegs = myBrain.legs;
		seekBehaviour = new Arrive();
		seekBehaviour.Init (myLegs);
		separation = new Separation();
		separation.Init (myLegs);
		cohesion = new Cohesion();
		cohesion.Init (myLegs);
		alignment = new Alignment();
		alignment.Init (myLegs);
		random = new RandomWalk(0.6f, 1, 2);
		random.Init (myLegs);
		// By name
    	var go = GameObject.Find("SheepTarget");
		seekBehaviour.setTarget (go);
		myLegs.addSteeringBehaviour(seekBehaviour);
		myLegs.addSteeringBehaviour(separation);
		myLegs.addSteeringBehaviour(cohesion);
		myLegs.addSteeringBehaviour(alignment);
		myLegs.addSteeringBehaviour(random);
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
        return "Setup Steering";
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
