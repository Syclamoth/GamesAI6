using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_eating : State
{

    public ExplicitStateReference roam = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    Flee fleeBehaviour;
    Arrive arriveBehaviour;

    private SensedObject sheepTarget;
    private Memory sheepMemory;
    private Brain sheepBrain;


    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;

        //get sheep target
        sheepTarget = myBrain.memory.GetValue<SensedObject>("hasCommand");

        sheepBrain = (Brain)sheepTarget.getObject().GetComponent("Brain");
        sheepMemory = sheepBrain.memory;
        sheepMemory.SetValue("BeingEaten", true);

        fleeBehaviour = new Flee();
        arriveBehaviour = new Arrive();

        fleeBehaviour.Init(myLeg);
        arriveBehaviour.Init(myLeg);

        myLeg.addSteeringBehaviour(fleeBehaviour);
        fleeBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));
        arriveBehaviour.setTarget(sheepTarget.getObject());

        //speed is zero
        myBrain.legs.maxSpeed = 0f;

        Debug.Log("I'm eating:" + sheepTarget.getObject() + " HP: " + sheepMemory.GetValue<float>("HP"));

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);
        
        //check if the sheep is still alive or not
        if (sheepTarget.getObject().active)
        {
            //delete itself in sheepTarget memory
            List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

            if (wolvesChasing.Count > 0)
            {
                wolvesChasing.Remove(this.myBrain);
                sheepMemory.SetValue("chasedBy", wolvesChasing);
            }
        }

        //delete its target
        myBrain.memory.SetValue("hasCommand", null);

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsShepherd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
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

        if (thereIsShepherd)
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() + Time.deltaTime / myBrain.memory.GetValue<float>("ferocity"));
            myBrain.legs.maxSpeed = 8f;
            if (fleeBehaviour.getWeight() > 15f)
            {
                fleeBehaviour.setWeight(15f);
            }
            else
            {
                sheepMemory.SetValue("HP", sheepMemory.GetValue<float>("HP") - (Time.deltaTime * myBrain.memory.GetValue<float>("damage") / 4));
            }
            Debug.Log("Flee away from Sheperd: " + fleeBehaviour.getWeight());
        }
        else
        {
            fleeBehaviour.setWeight(0f);
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * myBrain.memory.GetValue<float>("ferocity"));

            if (arriveBehaviour.getWeight() > 20f)
            {
                arriveBehaviour.setWeight(20f);
            }

            //if the wolf catches the sheep again
            float distance = Vector2.Distance(myBrain.legs.getPosition(), sheepBrain.legs.getPosition());

            if (distance < 0f)
            {
                distance = distance * (-1); //distance can't be negative
            }

            if (distance <= 2f)
            {
                myBrain.legs.maxSpeed = 0f;
                sheepMemory.SetValue("HP", sheepMemory.GetValue<float>("HP") - (Time.deltaTime * myBrain.memory.GetValue<float>("damage")));
                //Debug.Log("Sheep's HP is: " +  sheepMemory.GetValue<float>("HP"));
            }
        }

        if (sheepMemory.GetValue<float>("HP") <= 0f)
        {
            sheepTarget.getObject().SetActiveRecursively(false);

	        Debug.Log("I ate the sheep");
            myBrain.memory.SetValue("hungryLevel", myBrain.memory.GetValue<float>("hungryLevel") + 10f);
            mainMachine.RequestStateTransition(roam.GetTarget());
        }
        
        float dist = Vector2.Distance(myBrain.legs.getPosition(), sheepBrain.legs.getPosition());
        
        if (dist >= 8f)
        {
            Debug.Log("I can't eat the sheep");
            mainMachine.RequestStateTransition(roam.GetTarget());
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
        retV.Add(new LinkedStateReference(roam, "Lost Target"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Eating";
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
