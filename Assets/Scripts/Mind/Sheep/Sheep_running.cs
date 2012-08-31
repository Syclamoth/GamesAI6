using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_running : State {

    public ExplicitStateReference calmed = new ExplicitStateReference(null);
	public ExplicitStateReference nuts = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    private Arrive arriveBehaviour;
    private Flee fleeBehaviour;

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
        fleeBehaviour = new Flee();

        arriveBehaviour.Init(myLeg);
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

        if (sensedWolf.Count > 0)
        {
            //the less cowardLevel is, the less Panic increases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));

            foreach(SensedObject obj in sensedWolf)
            {
                fleeBehaviour.setTarget(obj.getObject());
                fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));
            }
        }
        else
        {
            //the less cowardLevel is, the more Panic decreases
            controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            
            //set the minimum Panic level for sheep
            if (controller.memory.GetValue<float>("Panic") < 0f)
            {
                controller.memory.SetValue("Panic", 0f);
                fleeBehaviour.setTarget(null);
            }

            //decrease flee weight
            fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic"));
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

        //if the sheep get caught
        if(controller.memory.GetValue<List<Brain>>("chasedBy").Count > 0)
        {
            foreach (Brain wolvesBrain in controller.memory.GetValue<List<Brain>>("chasedBy"))
            {
                Vector2 currentHunterPos = wolvesBrain.legs.getPosition();
                Vector2 currentSheepPos = myBrain.legs.getPosition();

                float distance = Vector2.Distance(currentHunterPos, currentSheepPos);

                if (distance <= 1.5f)
                {
                    mainMachine.RequestStateTransition(eaten.GetTarget());
                }
            }
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
        retV.Add(new LinkedStateReference(eaten, "Eaten"));
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
