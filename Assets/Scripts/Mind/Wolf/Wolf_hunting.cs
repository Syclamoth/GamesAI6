using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_hunting : State
{

    public ExplicitStateReference roam = new ExplicitStateReference(null);
    public ExplicitStateReference eating = new ExplicitStateReference(null);

    private float time;

    Machine mainMachine;
    Brain myBrain;

    private SensedObject sheepTarget;
    private Memory sheepMemory;
    private Brain sheepBrain;

	private Arrive arriveBehaviour;
    private Seek seekBehaviour;
    private Flee fleeBehaviour;

    private float ferocityRate;
    private Vector2 oldTargetPosition;
    private float rateShepherd = 1f;


    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLeg = myBrain.legs;
		arriveBehaviour = new Arrive();
        seekBehaviour = new Seek();
        fleeBehaviour = new Flee();

     
        fleeBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));
		arriveBehaviour.setTarget(controller.memory.GetValue<SensedObject>("hasCommand").getObject());

		arriveBehaviour.Init(myLeg);
        seekBehaviour.Init(myLeg);
        fleeBehaviour.Init(myLeg);

		myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(seekBehaviour);
        myLeg.addSteeringBehaviour(fleeBehaviour);

        myLeg.maxSpeed = 10f;
        time = 0f;
        ferocityRate = controller.memory.GetValue<float>("ferocity");

        sheepTarget = myBrain.memory.GetValue<SensedObject>("hasCommand");
        sheepMemory = sheepTarget.getMemory();
        sheepBrain = (Brain)sheepTarget.getObject().GetComponent("Brain");
        yield return null;
    }
    public override IEnumerator Exit()
    {
		myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(seekBehaviour);
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);

        //delete itself in sheepTarget memory
        List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

        if(wolvesChasing.Count > 0)
        {
            wolvesChasing.Remove(this.myBrain);
            sheepMemory.SetValue("chasedBy", wolvesChasing);
        }      

        //delete its target
        myBrain.memory.SetValue("hasCommand", null);

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool stillSeeTarget = false;
        bool thereIsShepherd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if(obj.getObject().Equals(controller.memory.GetValue<SensedObject>("hasCommand").getObject()))
            {
                stillSeeTarget = true;
            }
            if(obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsShepherd = true;
            }
        }

        if (thereIsShepherd)
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() + Time.deltaTime);
            if (fleeBehaviour.getWeight() > 15f)
            {
                fleeBehaviour.setWeight(15f);
            }
            rateShepherd = 0.5f;
        }
        else
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() - Time.deltaTime);
            if (fleeBehaviour.getWeight() < 0f)
            {
                fleeBehaviour.setWeight(0f);
            }
            rateShepherd = 1f;
        }

        if (stillSeeTarget)
        {
            //increase weight based on ferocityRate and rateShepherd. If the wolf see the Sheperd, the thought to chase the sheep will be suppressed
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + (Time.deltaTime * ferocityRate * rateShepherd));

            seekBehaviour.setWeight(seekBehaviour.getWeight() - (Time.deltaTime * ferocityRate * rateShepherd));

            if (arriveBehaviour.getWeight() > 15f)
            {
                arriveBehaviour.setWeight(15f);
            }
            if (seekBehaviour.getWeight() < 0f)
            {
                seekBehaviour.setWeight(0f);
            }
            oldTargetPosition = arriveBehaviour.getTarget();
        }
        else
        {
            time += Time.deltaTime;
            if (time > 7f)
            {
                //keep chasing
                seekBehaviour.setTarget(oldTargetPosition);
                seekBehaviour.setWeight(seekBehaviour.getWeight() + Time.deltaTime * ferocityRate);

                //arrive's weight decreases
                arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * (5f - ferocityRate)));

                if (seekBehaviour.getWeight() > 15f)
                {
                    seekBehaviour.setWeight(15f);
                }
                if (arriveBehaviour.getWeight() < 0f)
                {
                    arriveBehaviour.setWeight(0f);
                }
            }

            if (arriveBehaviour.getWeight() == 0f)
            {
                //change to roaming state
                Debug.Log("Give up finding");
                mainMachine.RequestStateTransition(roam.GetTarget());
            }
        }

        //if the wolf catches the sheep
        Vector2 currentHunterPos = myBrain.legs.getPosition();
        Vector2 currentSheepPos = sheepBrain.legs.getPosition();

        float distance = Vector2.Distance(currentHunterPos, currentSheepPos);

        if (distance <= 1.5f)
        {
            mainMachine.RequestStateTransition(eating.GetTarget());
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
        retV.Add(new LinkedStateReference(eating, "Eating"));
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
