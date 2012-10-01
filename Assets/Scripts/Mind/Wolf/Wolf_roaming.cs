using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_roaming : State {

    public ExplicitStateReference hunting = new ExplicitStateReference(null);

    private float time = 0f;
    private SensedObject target;

    private float decayLeaderLevel = 0.2f;
    private float decayHungryLevel = 0.1f;
    private float increaseLeaderLevel = 15f;
    private float decreaseLeaderLevel = 3f;
	
	private bool firstActivation = true;
	
    Machine mainMachine;
    Brain myBrain;

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

            myBrain.memory.SetValue("ferocity", Random.value * 5);

            if (myBrain.memory.GetValue<float>("ferocity") < 1f)
            {
                myBrain.memory.SetValue("ferocity", 1f);
            }
			
			myBrain.memory.SetValue ("shouldHide", 0f);
			
            firstActivation = false;
        }
        else
        {
            Debug.Log("I'm roaming");

            //delete its target
            myBrain.memory.SetValue("hasCommand", null);
        }

        myLeg.maxSpeed = 8.0f;
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
                PlayerLegs playerLeg = (PlayerLegs)obj.getObject().GetComponent<PlayerLegs>();
                Vector2 playerPos = playerLeg.getPosition();
                Vector2 wolfPos = myBrain.legs.getPosition();
                Vector2 playerFacing = new Vector2(playerLeg.transform.forward.x, playerLeg.transform.forward.z);

                float dot = Vector2.Dot(playerFacing, wolfPos - playerPos);

                if (dot > 0)
                {
                    thereIsShepherd = true;
                }
            }
        }
            //check if this wolf has been given command to attack or not
            if (controller.memory.GetValue<SensedObject>("hasCommand") != null)
            {
                //decrease leaderLevel because it has been given command by others
                controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - decreaseLeaderLevel);

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
                    target = seenSheep[(int)Random.Range(0, seenSheep.Count)];

                    //target is alive
				    if(target.getObject().GetComponent<Brain>().memory.GetValue<float>("HP") > 0)
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
		
		            	//Change to hunting phase
		            	Debug.Log("I'm hunting. Target: " + controller.memory.GetValue<SensedObject>("hasCommand").getObject());
		    		}
		        }
		    }

        if (controller.memory.GetValue<SensedObject>("hasCommand") != null)
        {
            time = 0f;
            if (controller.memory.GetValue<float>("hungryLevel") <= 4f)
            {
                //it doesn't have enough hungryLevel to operate. Die!
                controller.memory.SetValue("hungryLevel", 0f);
                Debug.Log("I died because don't have enough hungryLevel");
                myBrain.getGameObject().SetActiveRecursively(false);
            }
            else
            {
                //decrease its hungryLevel when it change to hunting state
                controller.memory.SetValue("hungryLevel", controller.memory.GetValue<float>("hungryLevel") - 4f); 
                mainMachine.RequestStateTransition(hunting.GetTarget());
            }
        }

        else
        {
            if (time >= 30f) //wait for 30 sec
            {
                //decrease its leaderLevel if it can't find any sheep or cant issue and command
                if (controller.memory.GetValue<float>("leaderLevel") > 10f)
                {
                    controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - (decayLeaderLevel / myBrain.memory.GetValue<float>("ferocity")));
                }

                if (controller.memory.GetValue<float>("hungryLevel") > 0f)
                {
                    myBrain.memory.SetValue("hungryLevel", controller.memory.GetValue<float>("hungryLevel") - (decayHungryLevel / myBrain.memory.GetValue<float>("ferocity")));
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

        yield return null;
    }
	
	void UpdateCaution() {
		/*
		Transform player = 
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		if(Vector2.Dot(playerFacing, positionOffset) > 0.71f) {
		}
		*/
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
}
