using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_running : State {

    public ExplicitStateReference calmed = new ExplicitStateReference(null);
	public ExplicitStateReference nuts = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    private Pathfind arriveBehaviour;

    private Flee fleeBehaviour;

    private float decayFollowRate = 0.5f;
    private float increaseFollowRate = 3f;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 4f;

    private float shepherdInfluence = 1f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;
        
        arriveBehaviour = new Pathfind();
        fleeBehaviour = new Flee();

        arriveBehaviour.Init(myLeg, myBrain.levelGrid);
        fleeBehaviour.Init(myLeg);
        myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(fleeBehaviour);

        //inscrease speed
        myBrain.legs.maxSpeed = 9f;

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        List<SensedObject> sensedWolf = new List<SensedObject>();

        bool thereIsShepherd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                sensedWolf.Add(obj);
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsShepherd = true;
            }
        }

        if (thereIsShepherd)
        {
            //set the target
            arriveBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));

            //set the weight, this is top priority
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * increaseFollowRate);
            //set maximum weight
            if (arriveBehaviour.getWeight() > 15f)
            {
                arriveBehaviour.setWeight(15f);
            }
            shepherdInfluence = 2f;
        }
        else
        {
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * decayFollowRate));
            //set minimum weight
            if (arriveBehaviour.getWeight() < 0f)
            {
                arriveBehaviour.setWeight(0f);
            }
            shepherdInfluence = 1f;
        }

        if (sensedWolf.Count > 0)
        {
            //the less cowardLevel is, the less Panic increases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * (1 - increasePanicRate) * controller.memory.GetValue<float>("cowardLevel")));

            foreach(SensedObject obj in sensedWolf)
            {
                fleeBehaviour.setTarget(obj.getObject());
                fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));
            }

            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
                fleeBehaviour.setTarget(null);
            }
        }
        else
        {
            //the less cowardLevel is, the more Panic decreases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * shepherdInfluence * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            
            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
                fleeBehaviour.setTarget(null);
            }

            //decrease flee weight
            fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));
        }        

        //if the sheep get caught
        if (controller.memory.GetValue<float>("Panic") >= 55f)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
        }
        
        // if panic level larger than 30, change to gonenuts state.
        if (controller.memory.GetValue<float>("Panic") >= 25f)
        {
            Debug.Log("I'm so scared!");
            mainMachine.RequestStateTransition(nuts.GetTarget());
        }
        // if can't see wolf and panic level has decreased, change to roaming state
        else if (controller.memory.GetValue<float>("Panic") < 7f)
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
