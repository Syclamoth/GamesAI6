using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_running : State {

    public ExplicitStateReference calmed = new ExplicitStateReference(null);
	public ExplicitStateReference nuts = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    private PathfindToPoint arriveBehaviour;

    private Flee fleeBehaviour;

    private float decayFollowRate = 0.5f;
    private float increaseFollowRate = 3f;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 6f;

    private float shepherdInfluence = 3f;
    private float time = 0f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;
        
        arriveBehaviour = new PathfindToPoint();
        fleeBehaviour = new Flee();

        arriveBehaviour.Init(myLeg, myBrain.levelGrid);
        fleeBehaviour.Init(myLeg);
        myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(fleeBehaviour);

        //inscrease speed
        myBrain.legs.maxSpeed = 9f;
        time = 0f;

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

        bool thereIsSheperd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Wolf))
            {
                Legs wolfLeg = (Legs)obj.getObject().GetComponent<Legs>();
                Vector2 wolfFacing = new Vector2(wolfLeg.transform.forward.x, wolfLeg.transform.forward.z);
                Vector2 sheepPos = myBrain.legs.getPosition();
                Vector2 wolfPos = wolfLeg.getPosition();

                float dot = Vector2.Dot(wolfFacing, sheepPos - wolfPos);

                if (dot > 0)
                {
                    sensedWolf.Add(obj);
                }
                if (controller.memory.GetValue<List<Brain>>("chasedBy") != null)
                {
                    List<Brain> chaseByList = (List<Brain>)controller.memory.GetValue<List<Brain>>("chasedBy");
                    Brain objBrain = (Brain)obj.getObject().GetComponent<Brain>();

                    if (chaseByList.Contains(objBrain))
                    {
                        sensedWolf.Add(obj);
                    }
                }
            }

            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
                thereIsSheperd = true;
            }
        }

        if (thereIsSheperd)
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

        if (sensedWolf.Count > 0)
        {
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * sensedWolf.Count + increasePanicRate / shepherdInfluence * controller.memory.GetValue<float>("cowardLevel")));
            }
            else
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * sensedWolf.Count + increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
            }
            foreach(SensedObject obj in sensedWolf)
            {
                fleeBehaviour.setTarget(obj.getObject());
                fleeBehaviour.setWeight(controller.memory.GetValue<float>("Panic") * 1.5f);
            }
        }
        else
        {
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the more Panic decreases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * shepherdInfluence * (1 - controller.memory.GetValue<float>("cowardLevel"))));
                time = 0f;
            }
            else
            {
                if (time > 7f)
                {
                    //the less cowardLevel is, the more Panic decreases
                    controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
                }
                else
                {
                    time += Time.deltaTime;
                }                
            }
            
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
        if (controller.memory.GetValue<bool>("BeingEaten") == true)
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
        else if (controller.memory.GetValue<float>("Panic") < 2f)
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
