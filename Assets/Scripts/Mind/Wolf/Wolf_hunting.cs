using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf_hunting : State
{

    public ExplicitStateReference alarm = new ExplicitStateReference(null);

    private float time;

    Machine mainMachine;
    Brain myBrain;

	private Arrive arriveBehaviour;
    private Seek seekBehaviour;
    private float ferocityRate;
    private Vector2 oldTargetPosition;


    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLeg = myBrain.legs;
		arriveBehaviour = new Arrive();
        seekBehaviour = new Seek();

		arriveBehaviour.setTarget(controller.memory.GetValue<SensedObject>("hasCommand").getObject());
		arriveBehaviour.Init(myLeg);
        seekBehaviour.Init(myLeg);

		myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(seekBehaviour);

        myLeg.maxSpeed = 10f;
        time = 0f;
        ferocityRate = controller.memory.GetValue<float>("ferocity");

        yield return null;
    }
    public override IEnumerator Exit()
    {
		myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(seekBehaviour);

        //delete its target
        myBrain.memory.SetValue("hasCommand", null);   
        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool stillSeeTarget = false;
        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if(obj.getObject().Equals(controller.memory.GetValue<SensedObject>("hasCommand").getObject()))
            {
                stillSeeTarget = true;
            }
        }

        if (stillSeeTarget)
        {
            //increase weight based on ferocityRate
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * ferocityRate);

            if (arriveBehaviour.getWeight() > 20f)
            {
                arriveBehaviour.setWeight(20f);
            }

            seekBehaviour.setWeight(seekBehaviour.getWeight() - Time.deltaTime * ferocityRate);

            if (seekBehaviour.getWeight() < 0f)
            {
                seekBehaviour.setWeight(0f);
            }
            oldTargetPosition = arriveBehaviour.getTarget();
        }
        else
        {
            time += Time.deltaTime;
            if (time > 7f)
            {
                //keep chasing
                seekBehaviour.setTarget(oldTargetPosition);
                seekBehaviour.setWeight(seekBehaviour.getWeight() + Time.deltaTime * ferocityRate);

                //arrive's weight decreases
                arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * (5f - ferocityRate)));

                if (seekBehaviour.getWeight() > 10f)
                {
                    seekBehaviour.setWeight(10f);
                }
                if (arriveBehaviour.getWeight() < 0f)
                {
                    arriveBehaviour.setWeight(0f);
                }
            }

            if (arriveBehaviour.getWeight() == 0f)
            {
                //change to roaming state
                Debug.Log("Give up finding");
                mainMachine.RequestStateTransition(alarm.GetTarget());
            }
        }

        /*
        //if the wolf can't see his target anymore, chase it for 10sec to gain vision, if it still can't find the sheep, change to roaming state.
        if (controller.senses.GetSensedObjects().Contains(controller.memory.GetValue<SensedObject>("hasCommand")))
        {
            //increase weight based on ferocityRate
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * ferocityRate);

            if (arriveBehaviour.getWeight() > 20f)
            {
                arriveBehaviour.setWeight(20f);
            }

            seekBehaviour.setWeight(seekBehaviour.getWeight() - Time.deltaTime * ferocityRate);

            if (seekBehaviour.getWeight() < 0f)
            {
                seekBehaviour.setWeight(0f);
            }
            Debug.Log("still seeing sheep");
            oldTargetPosition = arriveBehaviour.getTarget();
        }
        else
        {
            Debug.Log("cant see sheep");
            time += Time.deltaTime;
            if (time > 7f)
            {
                //keep chasing
                seekBehaviour.setTarget(oldTargetPosition);
                seekBehaviour.setWeight(seekBehaviour.getWeight() + Time.deltaTime * ferocityRate);

                //arrive's weight decreases
                arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime * (5f - ferocityRate)));

                if (seekBehaviour.getWeight() > 10f)
                {
                    seekBehaviour.setWeight(10f);
                }
                if(arriveBehaviour.getWeight() < 0f)
                {
                    arriveBehaviour.setWeight(0f);
                }
            }

            if (arriveBehaviour.getWeight() == 0f)
            {
                //change to roaming state
                mainMachine.RequestStateTransition(alarm.GetTarget());
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
        retV.Add(new LinkedStateReference(alarm, "Lost Target"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Hunting";
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
