using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_eating : State
{

    public ExplicitStateReference roam = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    Flee fleeBehaviour;
    Arrive arriveBehaviour;

    private SensedObject sheepTarget;
    private Memory sheepMemory;
    private Brain sheepBrain;


    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;

        //get sheep target
        sheepTarget = myBrain.memory.GetValue<SensedObject>("hasCommand");
        Debug.Log("I'm eating:" + sheepTarget.getObject());

        sheepBrain = (Brain)sheepTarget.getObject().GetComponent("Brain");
        sheepMemory = sheepBrain.memory;

        fleeBehaviour = new Flee();
        arriveBehaviour = new Arrive();

        fleeBehaviour.Init(myLeg);
        arriveBehaviour.Init(myLeg);

        myLeg.addSteeringBehaviour(fleeBehaviour);
        fleeBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));
        arriveBehaviour.setTarget(sheepTarget.getObject());

        //speed is zero
        myBrain.legs.maxSpeed = 0f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);
        
        //check if the sheep is still alive or not
        if (sheepTarget.getObject().active)
        {
            //delete itself in sheepTarget memory
            List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

            if (wolvesChasing.Count > 0)
            {
                wolvesChasing.Remove(this.myBrain);
                sheepMemory.SetValue("chasedBy", wolvesChasing);
            }
        }

        //delete its target
        myBrain.memory.SetValue("hasCommand", null);

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsShepherd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsShepherd = true;
            }
        }

        if (thereIsShepherd)
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() + Time.deltaTime);
            myBrain.legs.maxSpeed = 6f;
            if (fleeBehaviour.getWeight() > 15f)
            {
                fleeBehaviour.setWeight(15f);
            }
        }
        else
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() - Time.deltaTime);
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime);

            if (fleeBehaviour.getWeight() < 0f)
            {
                fleeBehaviour.setWeight(0f);
            }
            if (arriveBehaviour.getWeight() > 7f)
            {
                arriveBehaviour.setWeight(7f);
            }

            //if the wolf catches the sheep
            Vector2 currentHunterPos = myBrain.legs.getPosition();
            Vector2 currentSheepPos = sheepBrain.legs.getPosition();

            float distance = Vector2.Distance(currentHunterPos, currentSheepPos);

            if (distance <= 1f)
            {
                myBrain.legs.maxSpeed = 0f;
            }
        }

        if (sheepMemory.GetValue<float>("Panic") >= 65f)
        {
            sheepTarget.getObject().active = false;
            mainMachine.RequestStateTransition(roam.GetTarget());
        }
        else if (controller.memory.GetValue<float>("Panic") < 50f)
        {
            mainMachine.RequestStateTransition(roam.GetTarget());
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
        retV.Add(new LinkedStateReference(roam, "Lost Target"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Eating";
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
