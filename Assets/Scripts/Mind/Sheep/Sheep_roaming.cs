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

    private float decayFollowRate = 0.8f;
    private float increaseFollowRate = 3f;
    
    private float decayPanicRate = 1.5f;
    private float increasePanicRate = 6f;

    private float shepherdInfluence = 3f;
    private float time = 0f;
    private float seekingTime = 0f;
	
	private float fleeThreshold = 5f;
    private BeaconInfo curBeacon = null;
	
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
            float coward = 0.5f;

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
        Transform playerTrans = null;
		
		float averagePanicLevel = controller.memory.GetValue<float>("Panic");
		int totalSheep = 1;
        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                
                seenWolf.Add(obj);
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                playerTrans = obj.getObject().transform;
                thereIsSheperd = true;
            }
			
			// Let the the crowd influence the mood of individual sheep. This will make single scared sheep less panicky, but allow mass confusion to ensue given enough panic.
			if (obj.getAgentType() == AgentClassification.Sheep) {
				averagePanicLevel += obj.getObject().GetComponent<Brain>().memory.GetValue<float>("Panic");
				totalSheep++;
			}
			
        }

        //get current BeaconInfo
        if (controller.memory.GetValue<BeaconInfo>("LastBeacon") != null)
        {
            if (curBeacon != null)
            {
                if (curBeacon.GetTime() <= controller.memory.GetValue<BeaconInfo>("LastBeacon").GetTime())
                {
                    curBeacon = controller.memory.GetValue<BeaconInfo>("LastBeacon");
                }
            }
            else
            {
                curBeacon = controller.memory.GetValue<BeaconInfo>("LastBeacon");
            }
        }

        if (curBeacon != null)
        {
            controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") - 0.3f);

            controller.memory.SetValue<float>("Panic", 0f);
        }

        if (controller.memory.GetValue<float>("cowardLevel") <= 0f)
        {
            controller.memory.SetValue("cowardLevel", 0.01f);
        }
        else if (controller.memory.GetValue<float>("cowardLevel") >= 1f)
        {
            controller.memory.SetValue("cowardLevel", 0.99f);
        }

        if (thereIsSheperd)
        {
            Vector2 playerPos = new Vector2(playerTrans.position.x, playerTrans.position.z);
            Vector2 playerFacing = new Vector2(playerTrans.forward.x, playerTrans.forward.z);
            Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;

            //decrease cowardLevel when there is sheperd looking at it
            if (Vector2.Dot(playerFacing.normalized, positionOffset.normalized) > 0.71f)
            {
                controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") - (Time.deltaTime * 0.02f));

                if (controller.memory.GetValue<float>("cowardLevel") <= 0f)
                {
                    controller.memory.SetValue("cowardLevel", 0.01f);
                }
            }
            //normalise cowardLevel back to normal if Sheperd isn't looking
            else
            {
                controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") + (Time.deltaTime * 0.01f));

                if (controller.memory.GetValue<float>("cowardLevel") >= 0.5f)
                {
                    controller.memory.SetValue("cowardLevel", 0.5f);
                }
            }
        }
        else
        {
            //decrease cowardLevel when there are more than 4 sheep around it.
            if (totalSheep >= 4)
            {
                controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") - (Time.deltaTime * 0.01f));

                if (controller.memory.GetValue<float>("cowardLevel") <= 0f)
                {
                    controller.memory.SetValue("cowardLevel", 0.01f);
                }
            }
            //normalise cowardLevel to its default value
            else
            {
                if (controller.memory.GetValue<float>("cowardLevel") > 0.5f)
                {
                    controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") - (Time.deltaTime * 0.01f));

                    if (controller.memory.GetValue<float>("cowardLevel") <= 0.5f)
                    {
                        controller.memory.SetValue("cowardLevel", 0.5f);
                    }
                }
                else
                {
                    controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") + (Time.deltaTime * 0.01f));

                    if (controller.memory.GetValue<float>("cowardLevel") >= 0.5f)
                    {
                        controller.memory.SetValue("cowardLevel", 0.5f);
                    }
                }
            }
        }

		averagePanicLevel /= totalSheep;
		
		controller.memory.SetValue<float>("Panic", Mathf.Lerp(controller.memory.GetValue<float>("Panic"), averagePanicLevel, Time.deltaTime * 0.3f));
		
		foreach(SensedObject obj in seenWolf) {
			Legs wolfLeg = (Legs)obj.getObject().GetComponent<Legs>();
            Vector2 wolfFacing = wolfLeg.getVelocity();
            Vector2 sheepPos = myBrain.legs.getPosition();
            Vector2 wolfPos = wolfLeg.getPosition();

            float dot = Vector2.Dot(wolfFacing, sheepPos - wolfPos);
			float distance = Vector2.Distance(sheepPos, wolfPos);
			float scariness = (4 / distance) * (dot + 1);
			
			controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * scariness * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
		}
		
	    if (thereIsSheperd)
	    {
	        //the less cowardLevel is, the more Panic decreases
	        controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * shepherdInfluence * (1 - controller.memory.GetValue<float>("cowardLevel"))));
	        time = 0f;
	    }
	    else
	    {
	        if (time > 5f)
	        {
	            //the less cowardLevel is, the more Panic decreases
	            controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
	        }
	        else
	        {
	            time += Time.deltaTime;
	        }
	    }
	
	    //set the minimum Panic level for sheep
	    if (controller.memory.GetValue<float>("Panic") < 0f)
	    {
	        controller.memory.SetValue<float>("Panic", 0f);
	    }

        if (thereIsSheperd)
        {
            //set the target
            arriveBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));

            //set the weight, this is top priority
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * increaseFollowRate);
			
			
			
            //set maximum weight- the sheep stop following if they are afraid (the player must catch them)
			float maxWeight = (fleeThreshold - controller.memory.GetValue<float>("Panic")) + 5; //plus 5 to make the sheep more responsible to follow the Sheperd
            if (arriveBehaviour.getWeight() > maxWeight)
            {
                arriveBehaviour.setWeight(maxWeight);
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

            float maxWeight = fleeThreshold - controller.memory.GetValue<float>("Panic");
            if (arriveBehaviour.getWeight() > maxWeight)
            {
                arriveBehaviour.setWeight(maxWeight);
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

        // if panic level larger than a given threshold, change to running state.
        if (controller.memory.GetValue<float>("Panic") >= fleeThreshold)
        {
            Debug.Log("I saw wolf. RUN!: " + myBrain.getGameObject());
            mainMachine.RequestStateTransition(running.GetTarget());
        }

        //if the sheep get caught
        if (controller.memory.GetValue<bool>("BeingEaten") == true)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
        }

        //    Debug.Log("Hey" +controller.memory.GetValue<BeaconInfo>("LastBeacon").ToString());
        //deleat BeaconInfo after using
        curBeacon = null;

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
