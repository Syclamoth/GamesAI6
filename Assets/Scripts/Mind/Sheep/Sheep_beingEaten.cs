using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_beingEaten : State
{

    public ExplicitStateReference nuts = new ExplicitStateReference(null);
    
    Machine mainMachine;
    Brain myBrain;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;

        //speed is zero
        myBrain.legs.maxSpeed = 0f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        //back to gonenut state
        if (controller.memory.GetValue<bool>("BeingEaten") == false)
        {
            controller.memory.SetValue("HP", 100f);
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
