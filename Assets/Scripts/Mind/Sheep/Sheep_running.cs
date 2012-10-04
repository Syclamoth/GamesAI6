using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_running : State {

    public ExplicitStateReference calmed = new ExplicitStateReference(null);
	public ExplicitStateReference nuts = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;

    private Flee fleeBehaviour;

    private float decayFollowRate = 0.8f;
    private float increaseFollowRate = 3f;

    private float decayPanicRate = 1.5f;
    private float increasePanicRate = 2f;

    private float shepherdInfluence = 3f;
    private float time = 0f;

    private float fleeThreshold = 5f;
    private BeaconInfo curBeacon = null;
    private float beaconHelper = 0f;
    private float StopCurBeacon = 0f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;
        
        fleeBehaviour = new Flee();

        fleeBehaviour.Init(myLeg);
        myLeg.addSteeringBehaviour(fleeBehaviour);

        //inscrease speed
        myBrain.legs.maxSpeed = 9f;
        time = 0f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);
        yield return null;
    }
	
	int Scariest(SensedObject wolf1, SensedObject wolf2) {
		Legs wolfLeg1 = wolf1.getObject().GetComponent<Brain>().legs;
        Vector2 sheepPos = myBrain.legs.getPosition();
        Vector2 wolfPos1 = wolfLeg1.getPosition();
		float distance1 = Vector2.Distance(sheepPos, wolfPos1);
		Vector2 wolfFacing1 = new Vector2(wolfLeg1.transform.forward.x, wolfLeg1.transform.forward.z);
        float relativeVelocity1 = Vector2.Dot(wolfFacing1, sheepPos - wolfPos1);
		
		float scariness1 = (1 / distance1) * (relativeVelocity1 + 1);
		
		Legs wolfLeg2 = wolf2.getObject().GetComponent<Brain>().legs;
        Vector2 wolfPos2 = wolfLeg2.getPosition();
		float distance2 = Vector2.Distance(sheepPos, wolfPos2);
		Vector2 wolfFacing2 = new Vector2(wolfLeg2.transform.forward.x, wolfLeg2.transform.forward.z);
        float relativeVelocity2 = Vector2.Dot(wolfFacing2, sheepPos - wolfPos2);
		
		float scariness2 = (1 / distance2) * (relativeVelocity2 + 1);
		
		if(scariness1 > scariness2) {
			return 1;
		}
		if(scariness1 < scariness2) {
			return -1;
		}
		return 0;
	}
    public override IEnumerator Run(Brain controller)
    {
        List<SensedObject> sensedWolf = new List<SensedObject>();

        bool thereIsSheperd = false;
        int totalSheep = 1;
        Transform playerTrans = null;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                sensedWolf.Add (obj);
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                playerTrans = obj.getObject().transform;
                thereIsSheperd = true;
            }
            // Let the the crowd influence the mood of individual sheep. This will make single scared sheep less panicky, but allow mass confusion to ensue given enough panic.
            if (obj.getAgentType() == AgentClassification.Sheep)
            {
                totalSheep++;
            }
        }
		SensedObject scariestWolf = myBrain.memory.GetValue<SensedObject>("Scariest");
		if(scariestWolf != null) {
			sensedWolf.Add(scariestWolf);
		}

        //get current BeaconInfo
        if (controller.memory.GetValue<BeaconInfo>("LastBeacon") != null)
        {
            if (curBeacon != null)
            {
                if (curBeacon.GetTime() < controller.memory.GetValue<BeaconInfo>("LastBeacon").GetTime())
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
            //StopCurBeacon++;
            //beaconHelper = 0.2f;
            //if (StopCurBeacon==1)
            //{
            //if(Time.time - curBeacon.GetTime() <= Time.deltaTime)
            //{
            controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") - beaconHelper);
            controller.memory.SetValue<float>("Panic", 0f);
            //Debug.Log(controller.getGameObject() + ": " + (controller.memory.GetValue<float>("cowardLevel")));

            if (controller.memory.GetValue<float>("cowardLevel") <= 0f)
            {
                controller.memory.SetValue("cowardLevel", 0.01f);
            }
            //}
            //}
            //delete curBeacon
            curBeacon = null;
            controller.memory.SetValue("LastBeacon", null);
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
                    controller.memory.SetValue("cowardLevel", 0.001f);
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
                if (sensedWolf.Count == 0)
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
                        controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") + (Time.deltaTime * 0.02f));

                        if (controller.memory.GetValue<float>("cowardLevel") >= 0.5f)
                        {
                            controller.memory.SetValue("cowardLevel", 0.5f);
                        }
                    }
                }
                else
                {
                    controller.memory.SetValue("cowardLevel", controller.memory.GetValue<float>("cowardLevel") + (Time.deltaTime * 0.01f * sensedWolf.Count));

                    if (controller.memory.GetValue<float>("cowardLevel") >= 1f)
                    {
                        controller.memory.SetValue("cowardLevel", 0.99f);
                    }
                }
            }
        }

		sensedWolf.Sort(Scariest);
		
        if (sensedWolf.Count > 0)
        {
			scariestWolf = sensedWolf[0];
			myBrain.memory.SetValue("Scariest", scariestWolf);
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") - ((Time.deltaTime * (sensedWolf.Count * 0.5f) * decayPanicRate * controller.memory.GetValue<float>("cowardLevel")) / shepherdInfluence));
            }
            else
            {
				Vector2 sheepPos = myBrain.legs.getPosition();
       		 	Vector2 wolfPos = scariestWolf.getObject().GetComponent<Brain>().legs.getPosition();
				float wolfDistance = Vector2.Distance(sheepPos, wolfPos);
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * sensedWolf.Count * increasePanicRate * controller.memory.GetValue<float>("cowardLevel") * (2 / wolfDistance)));
            }
            fleeBehaviour.setTarget(scariestWolf.getObject());
            fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));// * 1.25f);
        }
        else
        {
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue<float>("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * shepherdInfluence * (1 - controller.memory.GetValue<float>("cowardLevel"))));
                time = 0f;
            }
            else
            {
                if (time > 3f)
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
                fleeBehaviour.setTarget(null);
            }

            //fleeBehaviour weight
            fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));
        }        

        //if the sheep get caught
        if (controller.memory.GetValue<bool>("BeingEaten") == true)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
        }
        
        // if panic level larger than 30, change to gonenuts state.
        if (controller.memory.GetValue<float>("Panic") >= 20f)
        {
            Debug.Log("I'm so scared!");
            mainMachine.RequestStateTransition(nuts.GetTarget());
        }
        // if can't see wolf and panic level has decreased, change to roaming state
        else if (controller.memory.GetValue<float>("Panic") < fleeThreshold)
        {
            Debug.Log("Wolf's gone!");
            mainMachine.RequestStateTransition(calmed.GetTarget());
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
        retV.Add(new LinkedStateReference(calmed, "Calmed"));
        retV.Add(new LinkedStateReference(nuts, "Insane"));
        retV.Add(new LinkedStateReference(eaten, "Being Eaten"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Sheep Running";
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
