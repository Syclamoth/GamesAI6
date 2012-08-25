using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_running : State {

    public ExplicitStateReference calmed = new ExplicitStateReference(null);
	public ExplicitStateReference nuts = new ExplicitStateReference(null);
    //public SheepCharacteristics stats;

    Machine mainMachine;
    Brain myBrain;
    private Arrive arriveBehaviour;

    private float decayFollowRate = 0.5f;
    private float increaseFollowRate = 3f;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 4f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;
        arriveBehaviour = new Arrive();

        arriveBehaviour.Init(myLeg);
        myLeg.addSteeringBehaviour(arriveBehaviour);

        //inscrease speed
        myBrain.legs.maxSpeed = 9f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheperd = false;
        bool thereIsWolf = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                thereIsWolf = true;
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsSheperd = true;
            }
        }

        if (thereIsWolf)
        {
            //the less cowardLevel is, the less Panic increases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
        }
        else
        {
            //the less cowardLevel is, the more Panic decreases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));

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
        }
        else
        {
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * decayFollowRate));
            //set minimum weight
            if (arriveBehaviour.getWeight() < 0f)
            {
                arriveBehaviour.setWeight(0f);
            }
        }

        /*
        //see Shepherd, follow him
        if (controller.senses.isContainAgent(AgentClassification.Shepherd))
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
        }
        else
        {
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * decayFollowRate));
            //set minimum weight
            if (arriveBehaviour.getWeight() < 0f)
            {
                arriveBehaviour.setWeight(0f);
            }
        }

        if (controller.senses.isContainAgent(AgentClassification.Wolf))
        {

            //the less cowardLevel is, the less Panic increases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
        }
        else
        {
            //the less cowardLevel is, the more Panic decreases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));

            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
            }
        }
        */
        // if panic level larger than 30, change to gonenuts state.
        if ((float)controller.memory.GetValue("Panic") >= 25f)
        {
            Debug.Log("I'm so scared!");
            mainMachine.RequestStateTransition(nuts.GetTarget());
        }
        // if can't see wolf and panic level has decreased, change to roaming state
        else if ((float)controller.memory.GetValue("Panic") < 7f)
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
