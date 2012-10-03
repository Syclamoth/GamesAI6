using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Wolf_roaming : State {

    public ExplicitStateReference hunting = new ExplicitStateReference(null);

    private float time = 0f;
    private SensedObject target;

    private float decayLeaderLevel = 0.2f;
    private float decayHungryLevel = 0.05f;
	
	private float watchedLevelDecay = 1;
	private float cautionLevelDecay = 0.1f;
	private float fleeThreshold = 3;
	
    private float increaseLeaderLevel = 15f;
    private float decreaseLeaderLevel = 3f;

    private BeaconInfo curBeacon = null;
	
	private bool firstActivation = true;
	
    Machine mainMachine;
    Brain myBrain;

    //check target
    private float sheepHPLimit = 0f;
    private float sheepDistanceSheperdLimit = 0f;
    private float sheepChasedByLimit = 0f;
    //private float wolfHungerLimit = 0f;
    //private float sheepPanicLimit = 0f;
    private float sheepCourageLimit = 0f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;

        time = 0f;
        if (firstActivation)
        {
            target = null;//set leaderLevel for wolf
            myBrain.memory.SetValue("leaderLevel", Random.value * 100);
            myBrain.memory.SetValue("hasCommand", null);
            myBrain.memory.SetValue("hungryLevel", 60f);
            myBrain.memory.SetValue("damage", 20f);
			myBrain.memory.SetValue("caution", 10f);
			myBrain.memory.SetValue ("watched", 0f);

            myBrain.memory.SetValue("ferocity", Random.value * 3);

            if (myBrain.memory.GetValue<float>("ferocity") < 0.8f)
            {
                myBrain.memory.SetValue("ferocity", 0.8f);
            }
			
			myBrain.memory.SetValue ("shouldHide", 0f);
			
            firstActivation = false;
        }
        else
        {
            Debug.Log("I'm roaming");

            //delete its target
            myBrain.memory.SetValue("hasCommand", null);
            myBrain.memory.SetValue("targeting", null);
        }

        myLeg.maxSpeed = 8.0f;

        generatedModel();
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsShepherd = false;
        float highestLeaderLevel = 0f;
        List<SensedObject> seenSheep = new List<SensedObject>();
        List<SensedObject> seenWolf = new List<SensedObject>();
		Transform playerTrans = null;
        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Sheep))
            {
                seenSheep.Add(obj);
            }
            else if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                Memory wolfMemory = obj.getMemory();

                if (highestLeaderLevel <= wolfMemory.GetValue<float>("leaderLevel"))
                {
                    highestLeaderLevel = wolfMemory.GetValue<float>("leaderLevel");
                }
                seenWolf.Add(obj);
            }
            else if(obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
				playerTrans = obj.getObject().transform;
            	thereIsShepherd = true;
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
            controller.memory.SetValue("shouldHide", 3f);
        }

		
		myBrain.memory.SetValue("caution", myBrain.memory.GetValue<float>("caution") - cautionLevelDecay * Time.deltaTime);
		if(thereIsShepherd) {
			UpdateCaution(playerTrans);
		} else {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
		}
		
		myBrain.memory.SetValue("watched", Mathf.Clamp(myBrain.memory.GetValue<float>("watched"), 0, Mathf.Infinity));
		if(myBrain.memory.GetValue<float>("watched") > 0)
		{
			//Debug.Log (myBrain.memory.GetValue<float>("watched"));
		}
		
        //check if this wolf has been given command to attack or not
        if (controller.memory.GetValue<SensedObject>("hasCommand") != null)
        {
            //decrease leaderLevel because it has been given command by others
            controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - (decreaseLeaderLevel * Time.deltaTime));

            //set the minimum leaderLevel for wolf
            if (controller.memory.GetValue<float>("leaderLevel") < 10f)
            {
                controller.memory.SetValue("leaderLevel", 10f);
            }

            //Change to hunting phase
            Debug.Log("I've received command. I'm hunting! Target: " + controller.memory.GetValue<SensedObject>("hasCommand").getObject());
        }
        else
        {
            if (seenSheep.Count > 0)
            {
                //choose the target
                //if the wolf hasn't have his target, pick it
                target = chooseTarget(seenSheep);

                if (target != null)
                {
                    //target is alive
                    if (target.getObject().GetComponent<Brain>().memory.GetValue<float>("HP") > 0)
                    {
                        //set the target for this wolf
                        controller.memory.SetValue("hasCommand", target);

                        //calling sheep that it is being targeted
                        Memory sheepMemory = target.getMemory();

                        //get a list of wolves that are chasing this sheep
                        List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

                        //add itself in
                        if (wolvesChasing != null)
                        {
                            wolvesChasing.Add(this.myBrain);
                            sheepMemory.SetValue("chasedBy", wolvesChasing);
                        }

                        //send signal to other wolf in its sensing radius, tell them to change to hunting phase
                        if (controller.memory.GetValue<float>("leaderLevel") >= highestLeaderLevel)
                        {
                            //increase its leaderLevel whenever it issue a decision to hunt
                            if (controller.memory.GetValue<float>("leaderLevel") < 100f)
                            {
                                controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") + increaseLeaderLevel);
                            }

                            //set the maximum leaderLevel for wolf
                            if (controller.memory.GetValue<float>("leaderLevel") > 100f)
                            {
                                controller.memory.SetValue("leaderLevel", 100f);
                            }

                            //call other to change to hunting phase
                            foreach (SensedObject objWolf in seenWolf)
                            {
                                //give out command to attack the same target
                                Memory wolfMemory = objWolf.getMemory();

                                wolfMemory.SetValue("hasCommand", target);
                                Debug.Log("I'm the leader! I sent command!");
                            }
                        }

                        //Change to hunting phasemyBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
                        Debug.Log("I'm hunting. Target: " + controller.memory.GetValue<SensedObject>("hasCommand").getObject());
                    }
                }
	        }
	    }

        if (controller.memory.GetValue<SensedObject>("hasCommand") != null)
        {
            time = 0f;
            //decrease its hungryLevel when it change to hunting state
            //controller.memory.SetValue("hungryLevel", controller.memory.GetValue<float>("hungryLevel") - 4f); 
            mainMachine.RequestStateTransition(hunting.GetTarget());
        }

        else
        {
            if (time >= 20f) //wait for 20 sec
            {
                //decrease its leaderLevel if it can't find any sheep or cant issue and command
                if (controller.memory.GetValue<float>("leaderLevel") > 10f)
                {
                    controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - (decayLeaderLevel / myBrain.memory.GetValue<float>("ferocity")));
                }

                if (controller.memory.GetValue<float>("hungryLevel") > 0f)
                {
                    myBrain.memory.SetValue("hungryLevel", controller.memory.GetValue<float>("hungryLevel") - (decayHungryLevel * (myBrain.memory.GetValue<float>("ferocity") / 5)));
                }

                //set the minimum leaderLevel for wolf
                if (controller.memory.GetValue<float>("leaderLevel") < 10f)
                {
                    controller.memory.SetValue("leaderLevel", 10f);
                }
                
                //set the minimum hungryLevel for wolf
                if (controller.memory.GetValue<float>("hungryLevel") < 0f)
                {
                    controller.memory.SetValue("hungryLevel", 0f);
                    Debug.Log("I died because I'm too hungry");
                    myBrain.getGameObject().SetActiveRecursively(false);
                }
            }
            else
            {
                time += Time.deltaTime;
            }   
        }

        //deleat BeaconInfo after using
        curBeacon = null;

       yield return null;
    }
	
	void UpdateCaution(Transform player) {
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		//Debug.Log (myBrain.legs.getPosition());
		//Debug.Log(playerPos);
		
		//Debug.DrawLine(playerPos.ToWorldCoords(), playerPos.ToWorldCoords() + positionOffset.ToWorldCoords());
		//Debug.Log (Vector2.Dot(playerFacing.normalized, positionOffset.normalized));
		
		if(Vector2.Dot(playerFacing.normalized, positionOffset.normalized) > 0.71f) {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") + (Time.deltaTime * (1 / positionOffset.magnitude)));
		} else {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
		}
		if(myBrain.memory.GetValue<float>("watched") > (1 / myBrain.memory.GetValue<float>("caution")) * fleeThreshold) {
			myBrain.memory.SetValue ("shouldHide", 2f);
		}
	}
	
    public override ObservedVariable[] GetExposedVariables()
    {
        return new ObservedVariable[] {
		};
    }


    override public List<LinkedStateReference> GetStateTransitions()
    {
        List<LinkedStateReference> retV = new List<LinkedStateReference>();
        retV.Add(new LinkedStateReference(hunting, "Attack"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Roaming";
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

    //J48 decision tree based on WEKA model
    public void generatedModel()
    {
        string curFile = @"./trainedData.arff";
        StreamReader fileReader;
        List<string> lines = new List<string>();
        List<string> success = new List<string>();
        List<string> fail = new List<string>();

        float totalSheepHPFail = 0f;
        float totalDistanceWithSheperdFail = 0f;
        float totalChasedBy = 0f;
        float totalHungryLevel = 0f;
        float totalCourageLevel = 0f;
        float totalPanicLevel = 0f;

        if (File.Exists(curFile))
        {
            fileReader = new StreamReader(curFile);
            string line = "";
            while ((line = fileReader.ReadLine()) != null)
            {
                if (!line.Contains("@") || !line.Contains(""))
                {
                    lines.Add(line);
                    string[] atts = line.Split(',');
                    if (line.Contains("SUCCESS"))
                    {
                        success.Add(line);
                    }
                    else if (line.Contains("FAIL"))
                    {
                        fail.Add(line);
                        totalSheepHPFail += float.Parse(atts[6].ToString());
                        totalDistanceWithSheperdFail += float.Parse(atts[4].ToString());
                        totalChasedBy += float.Parse(atts[2].ToString());
                        totalHungryLevel += float.Parse(atts[5].ToString());
                        totalCourageLevel += float.Parse(atts[1].ToString());
                        totalPanicLevel += float.Parse(atts[0].ToString());
                    }
                }
            }

            sheepHPLimit = ((totalSheepHPFail / fail.Count) + 73.1193f) / 2f; //lesser means fail
            sheepDistanceSheperdLimit = (((totalDistanceWithSheperdFail / fail.Count) + 15.61399f) / 2f); //larger means success
            sheepChasedByLimit = (((totalChasedBy / fail.Count) + 5f) / 3f);
            sheepCourageLimit = (((totalCourageLevel / fail.Count) + 0.494561f) / 2f); //lesser means success

            //wolfHungerLimit = (((totalHungryLevel / fail.Count) + 60.79216f) / 2f); //lesser means fail
            //sheepPanicLimit = (((totalPanicLevel / fail.Count) + 6.863676f) / 2f); //lesser means success
        }
    }

    public SensedObject chooseTarget(List<SensedObject> seenSheep)
    {
        SensedObject target = null;

        foreach (SensedObject sheep in seenSheep)
        {
            Brain sheepBrain = sheep.getObject().GetComponent<Brain>();
            Memory sheepMem = sheepBrain.memory;

            float sheepHP = sheepMem.GetValue<float>("HP");
            float sheepDistanceWithSheperd = Vector2.Distance(sheepBrain.legs.getPosition(), ((Legs)GameObject.FindGameObjectWithTag("Player").GetComponent<Legs>()).getPosition());
            float sheepChasedBy = sheepBrain.memory.GetValue<List<Brain>>("chasedBy").Count;
            float sheepCourage = sheepMem.GetValue<float>("cowardLevel");

            if (sheepHP > sheepHPLimit)
            {
                if (sheepDistanceWithSheperd > sheepDistanceSheperdLimit)
                {
                    target = sheep;
                    break;
                }
                else
                {
                    if (sheepChasedBy > sheepChasedByLimit)
                    {
                        if (sheepCourage <= sheepCourageLimit)
                        {
                            target = sheep;
                            break;
                        }
                    }
                }
            }
        }

        return target;
    }
}
