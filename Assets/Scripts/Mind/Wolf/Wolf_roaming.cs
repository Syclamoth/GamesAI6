using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_roaming : State {

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    private int time;
    private SensedObject target;
    //public SheepCharacteristics stats;
	
	private bool firstActivation = false;
	
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
			firstActivation = false;
		}
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        //check if this wolf has been given command to attack or not
        if (controller.memory.GetValue<SensedObject>("hasCommand") != null)
        {
            //decrease leaderLevel because it has been given command by others
            controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - 0.25);

            //Change to hunting phase
            mainMachine.RequestStateTransition(alarm.GetTarget());
        }
        
        //if it hasn't been given command, keep roaming until it see a target
        else
        {
            if (controller.senses.isContainAgent(AgentClassification.Sheep))
            {
                //choose the target
                //if the wolf hasn't have his target, pick it
                target = controller.senses.GetSensedSheep()[(int)Random.Range(0, controller.senses.GetSensedSheep().Count)];

                //set the target for this wolf
                controller.memory.SetValue("hasCommand", target);

                //send signal to other wolf in its sensing radius, tell them to change to hunting phase
                if (controller.memory.GetValue<float>("leaderLevel") >= controller.senses.getHighestLeaderLevel(controller))
                {
                    //increase its leaderLevel whenever it issue a decision to hunt
                    if (controller.memory.GetValue<float>("leaderLevel") < 100)
                    {
                        controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") + 0.25);
                    }

                    //call other to change to hunting phase
                    foreach (SensedObject objWolf in controller.senses.GetSensedWolf())
                    {
                        //give out command to attack the same target
                        objWolf.getMemory().SetValue("hasCommand", target);
                    }
                }

                //Change to hunting phase
                mainMachine.RequestStateTransition(alarm.GetTarget());
            }
            else
            {
                if (time >= 7) //wait for 7 sec
                {
                    //decrease its leaderLevel if it can't find any sheep or cant issue and command
                    if (controller.memory.GetValue<float>("leaderLevel") > 10)
                    {
                        controller.memory.SetValue("leaderLevel", controller.memory.GetValue<float>("leaderLevel") - 0.25);
                    }
                }
                else
                {
                    time++;
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
