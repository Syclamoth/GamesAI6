using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_roaming : State {

    public ExplicitStateReference hunting = new ExplicitStateReference(null);

    private float time;
    private SensedObject target;
    private Flee fleeBehaviour;

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
        Legs myLeg = myBrain.legs;
        fleeBehaviour = new Flee();
        fleeBehaviour.Init(myLeg);

        myLeg.addSteeringBehaviour(fleeBehaviour);
        fleeBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));

        time = 0;
        if (firstActivation)
        {
            target = null;//set leaderLevel for wolf
            myBrain.memory.SetValue("leaderLevel", Random.value * 100);
            myBrain.memory.SetValue("hasCommand", null);

            myBrain.memory.SetValue("ferocity", Random.value * 4);

            if (myBrain.memory.GetValue<float>("ferocity") < 0.8f)
            {
                myBrain.memory.SetValue("ferocity", 0.8f);
            }

            firstActivation = false;
        }
        else
        {
            Debug.Log("I'm roaming");
        }

        myLeg.maxSpeed = 6.0f;

        //delete its target
        myBrain.memory.SetValue("hasCommand", null);
        

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);
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
                thereIsShepherd = true;
            }
        }

        if(thereIsShepherd)
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() + Time.deltaTime);
            if (fleeBehaviour.getWeight() > 15f)
            {
                fleeBehaviour.setWeight(15f);
            }
        }
        else
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() - Time.deltaTime);
            if(fleeBehaviour.getWeight() < 0f)
            {
                fleeBehaviour.setWeight(0f);
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
            mainMachine.RequestStateTransition(hunting.GetTarget());
        }
        else
        {
            if (seenSheep.Count > 0)
            {
                //choose the target
                //if the wolf hasn't have his target, pick it
                target = seenSheep[(int)Random.Range(0, seenSheep.Count)];

                //set the target for this wolf
                controller.memory.SetValue("hasCommand", target);

                //calling sheep that it is being targeted
                Memory sheepMemory = target.getMemory();

                //get a list of wolves that are chasing this sheep
                List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

                //add itself in
                wolvesChasing.Add(this.myBrain);
                sheepMemory.SetValue("chasedBy", wolvesChasing);

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
                mainMachine.RequestStateTransition(hunting.GetTarget());
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

                    //Debug.Log("I'm hungry: " + controller.memory.GetValue<float>("leaderLevel"));
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
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
