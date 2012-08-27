using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_roaming : State {

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    private float time;
    private SensedObject target;
    private float decayLeaderLevel = 0.2f;
    private float increaseLeaderLevel = 15f;
    private float decreaseLeaderLevel = 3f;
	
	private bool firstActivation = true;
	
    Machine mainMachine;
    Brain myBrain;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        time = 0;
		if(firstActivation)
		{
        	target = null;//set leaderLevel for wolf
        	myBrain.memory.SetValue("leaderLevel", Random.value * 100);
            myBrain.memory.SetValue("hasCommand", null);

            myBrain.memory.SetValue("ferocity", Random.value * 4);

            if (myBrain.memory.GetValue<float>("ferocity") < 0.2f)
            {
                myBrain.memory.SetValue("ferocity", 0.2f);
            }

			firstActivation = false;
		}

        myBrain.legs.maxSpeed = 6.0f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheep = false;
        float highestLeaderLevel = 0f;
        List<SensedObject> seenSheep = new List<SensedObject>();
        List<SensedObject> seenWolf = new List<SensedObject>();

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Sheep))
            {
                thereIsSheep = true;
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
            mainMachine.RequestStateTransition(alarm.GetTarget());
        }
        else
        {
            if (thereIsSheep)
            {
                //choose the target
                //if the wolf hasn't have his target, pick it\
                target = seenSheep[(int)Random.Range(0, seenSheep.Count)];

                //set the target for this wolf
                controller.memory.SetValue("hasCommand", target);

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
                mainMachine.RequestStateTransition(alarm.GetTarget());
            }
            else
            {
                if (time >= 8f) //wait for 8 sec
                {
                    //decrease its leaderLevel if it can't find any sheep or cant issue and command
                    if (controller.memory.GetValue<float>("leaderLevel") > 10f)
                    {
                        controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - decayLeaderLevel);
                    }

                    //set the minimum leaderLevel for wolf
                    if (controller.memory.GetValue<float>("leaderLevel") < 10f)
                    {
                        controller.memory.SetValue("leaderLevel", 10f);
                    }

                    Debug.Log("I'm hungry: " + controller.memory.GetValue<float>("leaderLevel"));
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
        }

        /*
        //if it hasn't been given command, keep roaming until it see a target
        else
        {
            if (controller.senses.isContainAgent(AgentClassification.Sheep))
            {
                //choose the target
                //if the wolf hasn't have his target, pick it\
                target = controller.senses.GetSensedSheep()[(int)Random.Range(0, controller.senses.GetSensedSheep().Count)];

                //set the target for this wolf
                controller.memory.SetValue("hasCommand", target);

                //send signal to other wolf in its sensing radius, tell them to change to hunting phase
                if (controller.memory.GetValue<float>("leaderLevel") >= controller.senses.getHighestLeaderLevel(controller))
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
                    foreach (SensedObject objWolf in controller.senses.GetSensedWolf())
                    {
                        //give out command to attack the same target
                        objWolf.getMemory().SetValue("hasCommand", target);
                        Debug.Log("I'm the leader! I sent command!");
                    }
                }

                //Change to hunting phase
                Debug.Log("I'm hunting. Target: " + controller.memory.GetValue<SensedObject>("hasCommand").getObject());
                mainMachine.RequestStateTransition(alarm.GetTarget());
            } 
			else
            {
                if (time >= 8f) //wait for 8 sec
                {
                    //decrease its leaderLevel if it can't find any sheep or cant issue and command
                    if (controller.memory.GetValue<float>("leaderLevel") > 10f)
                    {
                        controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - decayLeaderLevel);
                    }

                    //set the minimum leaderLevel for wolf
                    if (controller.memory.GetValue<float>("leaderLevel") < 10f)
                    {
                        controller.memory.SetValue("leaderLevel", 10f);
                    }

                    Debug.Log("I'm hungry: " + controller.memory.GetValue<float>("leaderLevel"));
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
        }
        */
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
        retV.Add(new LinkedStateReference(alarm, "Attack"));
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
