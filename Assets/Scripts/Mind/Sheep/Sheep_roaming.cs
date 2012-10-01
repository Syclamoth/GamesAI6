using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_roaming : State {
    public ExplicitStateReference running = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;

    private PathfindToPoint arriveBehaviour;
    private Seek seekBehaviour;

    private Vector2 oldPlayerPosition;

	private bool firstActivation = true;

    private float decayFollowRate = 0.5f;
    private float increaseFollowRate = 3f;
    
    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 6f;

    private float shepherdInfluence = 3f;
    private float time = 0f;
    private float seekingTime = 0f;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;

        arriveBehaviour = new PathfindToPoint();
        seekBehaviour = new Seek();
        
        arriveBehaviour.Init(myLeg,myBrain.levelGrid);
        seekBehaviour.Init(myLeg);

        myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(seekBehaviour);

        //set speed back to normal
        myBrain.legs.maxSpeed = 5f;

		if(firstActivation)
		{
            //set cowardLevel for sheep. Random.value returns a random number between 0.0 [inclusive] and 1.0 [inclusive]. 
            float coward = Random.value;

            if (coward < 0.2f)
            {
                coward = 0.2f;
            }

        	myBrain.memory.SetValue("cowardLevel", coward);
            myBrain.memory.SetValue("chasedBy", new List<Brain>());
            myBrain.memory.SetValue("BeingEaten", false);
            myBrain.memory.SetValue("HP", 100f);

			firstActivation = false;
		}

        time = 0f;
        seekingTime = 0f;
        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(seekBehaviour);
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheperd = false;
        List<SensedObject> seenWolf = new List<SensedObject>();

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                Legs wolfLeg = (Legs)obj.getObject().GetComponent<Legs>();
                Vector2 wolfFacing = new Vector2(wolfLeg.transform.forward.x, wolfLeg.transform.forward.z);
                Vector2 sheepPos = myBrain.legs.getPosition();
                Vector2 wolfPos = wolfLeg.getPosition();

                float dot = Vector2.Dot(wolfFacing, sheepPos - wolfPos);
                if (dot > 0)
                {
                    seenWolf.Add(obj);
                }
                if (controller.memory.GetValue<List<Brain>>("chasedBy") != null)
                {
                    List<Brain> chaseByList = (List<Brain>)controller.memory.GetValue<List<Brain>>("chasedBy");
                    Brain objBrain = (Brain)obj.getObject().GetComponent<Brain>();

                    if (chaseByList.Contains(objBrain))
                    {
                        seenWolf.Add(obj);
                    }
                }
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsSheperd = true;
            }
        }

        if (seenWolf.Count > 0)
        {
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * seenWolf.Count * increasePanicRate / shepherdInfluence * controller.memory.GetValue<float>("cowardLevel")));
            }
            else
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * seenWolf.Count * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
            }
        }
        else
        {
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * shepherdInfluence * (1 - controller.memory.GetValue<float>("cowardLevel"))));
                time = 0f;
            }
            else
            {
                if (time > 7f)
                {
                    //the less cowardLevel is, the more Panic decreases
                    controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
                }
                else
                {
                    time += Time.deltaTime;
                }
            }

            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
            }
        }

        if (thereIsSheperd)
        {
            //set the target
            arriveBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));

            //set the weight, this is top priority
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * increaseFollowRate);

            //set maximum weight
            if (arriveBehaviour.getWeight() > 20f)
            {
                arriveBehaviour.setWeight(20f);
            }

            oldPlayerPosition = arriveBehaviour.getTarget();

            //if the sheep sees the shephered, it stops seeking
            seekBehaviour.setWeight(seekBehaviour.getWeight() - (Time.deltaTime * decayFollowRate));

            //minimum 0 weight
            if (seekBehaviour.getWeight() < 0f)
            {
                seekBehaviour.setWeight(0f);
            }
            seekingTime = 0f;
        }
        else
        {
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * decayFollowRate));

            //get last player's location so the sheep will seek around that
            seekBehaviour.setTarget(oldPlayerPosition);
            seekBehaviour.setWeight(seekBehaviour.getWeight() + (Time.deltaTime * decayFollowRate));

            //maximum 10 weight
            if (seekBehaviour.getWeight() > 10f)
            {
                seekBehaviour.setWeight(10f);
            }

            if (seekingTime > 7f)
            {
                seekBehaviour.setWeight(0f);
            }
            else
            {
                seekingTime += Time.deltaTime;
            }

            //set minimum weight
            if (arriveBehaviour.getWeight() < 0f)
            {
                arriveBehaviour.setWeight(0f);
                seekBehaviour.setWeight(seekBehaviour.getWeight() - Time.deltaTime);

                if (seekBehaviour.getWeight() < 0f)
                {
                    seekBehaviour.setWeight(0f);
                }
            }
        }

        // if panic level larger than 5, change to running state.
        if (controller.memory.GetValue<float>("Panic") >= 2f)
        {
            Debug.Log("I saw wolf. RUN!: " + myBrain.getGameObject());
            mainMachine.RequestStateTransition(running.GetTarget());
        }

        //if the sheep get caught
        if (controller.memory.GetValue<bool>("BeingEaten") == true)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
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
        retV.Add(new LinkedStateReference(running, "Running"));
        retV.Add(new LinkedStateReference(eaten, "Being Eaten"));
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
