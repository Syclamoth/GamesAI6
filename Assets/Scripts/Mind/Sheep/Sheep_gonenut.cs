using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_gonenut : State {

    public ExplicitStateReference roaming = new ExplicitStateReference(null);
    public ExplicitStateReference eaten = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;

    private float decayPanicRate = 0.75f;
    private float increasePanicRate = 6f;

    private float shepherdInfluence = 3f;
    private float time = 0f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;

        //set speed to minimum
        myBrain.legs.maxSpeed = 1f;
        time = 0f;
        yield return null;
    }
    public override IEnumerator Exit()
    {
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsSheperd = false;
        List<SensedObject> sensedWolf = new List<SensedObject>();

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

        if (sensedWolf.Count > 0)
        {
            //if there is wolf around and it can see the Sheperd, the recover rate from Panic is similar to w.o wolf.
            if (thereIsSheperd)
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") - (Time.deltaTime * decayPanicRate * (1 - controller.memory.GetValue<float>("cowardLevel"))));
            }
            else
            {
                //the less cowardLevel is, the less Panic increases
                controller.memory.SetValue("Panic", controller.memory.GetValue<float>("Panic") + (Time.deltaTime * sensedWolf.Count * increasePanicRate * controller.memory.GetValue<float>("cowardLevel")));
            }


            if (controller.memory.GetValue<float>("Panic") >= 35f)
            {
                controller.memory.SetValue("Panic", 35f);
            }
        }
        else
        {
            //if there is no wolf around and it can see the Sheperd, the recover rate from Panic is tripled.
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
            }
        }

        // if can't see wolf and panic level has decreased, change to roaming state
        if (controller.memory.GetValue<float>("Panic") < 3f)
        {
            Debug.Log("Wolf's gone and I'm calm now!");
            mainMachine.RequestStateTransition(roaming.GetTarget());
        }

        //if the sheep get caught
        if (controller.memory.GetValue<bool>("BeingEaten") == true)
        {
            mainMachine.RequestStateTransition(eaten.GetTarget());
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
        retV.Add(new LinkedStateReference(roaming, "Roaming"));
        retV.Add(new LinkedStateReference(eaten, "Being Eaten"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Sheep Gonenut";
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
