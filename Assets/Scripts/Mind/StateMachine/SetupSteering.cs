using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetupSteering : State {
	public ExplicitStateReference done = new ExplicitStateReference(null);

    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;
	private Separation separation;
	private RandomWalk random;
	private BoxAvoidance avoidWalls;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLegs = myBrain.legs;
		separation = new Separation(controller.allObjects);
		separation.Init(myLegs);
		random = new RandomWalk(0.6f, 1, 20);
		random.Init (myLegs);
		avoidWalls = new BoxAvoidance(controller.boxes, 10);
		avoidWalls.Init (myLegs);
		myLegs.addSteeringBehaviour(separation);
		myLegs.addSteeringBehaviour(random);
		myLegs.addSteeringBehaviour(avoidWalls);
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
