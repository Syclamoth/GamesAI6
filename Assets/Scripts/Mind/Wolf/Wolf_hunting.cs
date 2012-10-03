using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Wolf_hunting : State
{

    public ExplicitStateReference roam = new ExplicitStateReference(null);
    public ExplicitStateReference eating = new ExplicitStateReference(null);

    private float time;

    Machine mainMachine;
    Brain myBrain;

    private SensedObject sheepTarget;
    private Memory sheepMemory;
    private Brain sheepBrain;

    private PathfindToPoint arriveBehaviour;
    private Seek seekBehaviour;

    private float ferocityRate;
    private Vector2 oldTargetPosition;
    
	private float watchedLevelDecay = 1;
	private float cautionLevelDecay = 0.1f;
	private float fleeThreshold = 3;

    private BeaconInfo curBeacon = null;
	
    bool stillSeeTarget;
    private string result = "FAIL";
    private float decayHungryLevel = 0.05f;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLeg = myBrain.legs;
        arriveBehaviour = new PathfindToPoint();
        seekBehaviour = new Seek();

		arriveBehaviour.Init(myLeg, myBrain.levelGrid);
        seekBehaviour.Init(myLeg);

		myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(seekBehaviour);

        myLeg.maxSpeed = 11f;
        time = 0f;
        ferocityRate = controller.memory.GetValue<float>("ferocity");

        sheepTarget = myBrain.memory.GetValue<SensedObject>("hasCommand");
        sheepMemory = sheepTarget.getMemory();
        sheepBrain = (Brain)sheepTarget.getObject().GetComponent("Brain");

        arriveBehaviour.setTarget(sheepTarget.getObject());

        //for machine learning
        float panic = sheepBrain.memory.GetValue<float>("Panic");
        float courage = sheepBrain.memory.GetValue<float>("cowardLevel");
        float chasedBy = sheepBrain.memory.GetValue<List<Brain>>("chasedBy").Count;
        float distanceWithMe = Vector2.Distance(sheepBrain.legs.getPosition(), myBrain.legs.getPosition());
        float distanceWithSheperd = Vector2.Distance(sheepBrain.legs.getPosition(), ((Legs)GameObject.FindGameObjectWithTag("Player").GetComponent<Legs>()).getPosition());
        float hungry = myBrain.memory.GetValue<float>("hungryLevel");
        float sheepHP = sheepBrain.memory.GetValue<float>("HP");

        string sheep = panic + "," + courage + "," + chasedBy + "," + distanceWithMe + "," + distanceWithSheperd + "," + hungry + "," + sheepHP;

        myBrain.memory.SetValue("targeting", sheep);

        yield return null;
    }
    public override IEnumerator Exit()
    {
		myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(seekBehaviour);

        if (result == "FAIL")
        {
            createARFF(myBrain.memory.GetValue<string>("targeting"), result);
        }

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        List<GameObject> seenSheep = new List<GameObject>();

        bool thereIsShepherd = false;
		
		Transform playerTrans = null;
        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if(obj.getAgentType().Equals(AgentClassification.Sheep))
            {
                seenSheep.Add(obj.getObject());
            }
            if(obj.getAgentType().Equals(AgentClassification.Shepherd))
            {
				playerTrans = obj.getObject().transform;
            	thereIsShepherd = true;
            }
        }

        //get current BeaconInfo
        if (controller.memory.GetValue<BeaconInfo>("LastBeacon") != null)
        {
            if (curBeacon != null)
            {
                if (curBeacon.GetTime() <= controller.memory.GetValue<BeaconInfo>("LastBeacon").GetTime())
                {
                    curBeacon = controller.memory.GetValue<BeaconInfo>("LastBeacon");
                }
            }
            else
            {
                curBeacon = controller.memory.GetValue<BeaconInfo>("LastBeacon");
            }
        }

        if (curBeacon != null)
        {
            controller.memory.SetValue("shouldHide", 3f);
        }

        //increase its decayHungryLevel when hunting
        if (controller.memory.GetValue<float>("hungryLevel") > 0f)
        {
            controller.memory.SetValue("hungryLevel", controller.memory.GetValue<float>("hungryLevel") - (decayHungryLevel * 2 * (myBrain.memory.GetValue<float>("ferocity") / 5)));
        }
        else
        {
            controller.memory.SetValue("hungryLevel", 0f);
            Debug.Log("I died because I used up my energy");
            createARFF(myBrain.memory.GetValue<string>("targeting"), "FAIL");

            //delete itself in sheepTarget memory
            List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

            if (wolvesChasing.Count > 0)
            {
                wolvesChasing.Remove(this.myBrain);
                sheepMemory.SetValue("chasedBy", wolvesChasing);
            }

            myBrain.getGameObject().SetActiveRecursively(false);
        }
		
		myBrain.memory.SetValue("caution", myBrain.memory.GetValue<float>("caution") - cautionLevelDecay * Time.deltaTime);
		if(thereIsShepherd) {
			UpdateCaution(playerTrans);
		} else {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
		}
		
		myBrain.memory.SetValue("watched", Mathf.Clamp(myBrain.memory.GetValue<float>("watched"), 0, Mathf.Infinity));

        if (seenSheep.Count > 0)
        {
            if(seenSheep.Contains(myBrain.memory.GetValue<SensedObject>("hasCommand").getObject()))
            {
                stillSeeTarget = true;
            }
            else
            {
                stillSeeTarget = false;
            }
        }
        else
        {
            stillSeeTarget = false;
        }
        if (stillSeeTarget)
        {
            //increase weight based on ferocityRate.
            arriveBehaviour.setWeight(arriveBehaviour.getWeight() + (Time.deltaTime * ferocityRate));
            seekBehaviour.setWeight(0f);

            if (arriveBehaviour.getWeight() > 10f)
            {
                arriveBehaviour.setWeight(10f);
            }
            oldTargetPosition = arriveBehaviour.getTarget();
            time = 0f;
        }
        else
        {
            time += Time.deltaTime;
            if (time > 1.5f)
            {
                //keep chasing
                seekBehaviour.setTarget(oldTargetPosition);
                seekBehaviour.setWeight(seekBehaviour.getWeight() + (Time.deltaTime * ferocityRate));

                //arrive's weight decreases
                arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime / ferocityRate));

                if (seekBehaviour.getWeight() > 5f)
                {
                    seekBehaviour.setWeight(5f);
                }
                if (arriveBehaviour.getWeight() < 0f)
                {
                    arriveBehaviour.setWeight(0f);
                }
            }

            if (arriveBehaviour.getWeight() == 0f)
            {
                //change to roaming state
                UnityEngine.Debug.Log("Give up finding");

                //delete itself in sheepTarget memory
                List<Brain> wolvesChasing = sheepMemory.GetValue<List<Brain>>("chasedBy");

                if (wolvesChasing.Count > 0)
                {
                    wolvesChasing.Remove(this.myBrain);
                    sheepMemory.SetValue("chasedBy", wolvesChasing);
                }
                result = "FAIL";

                mainMachine.RequestStateTransition(roam.GetTarget());
            }
        }

        //if the wolf catches the sheep
        float distance = Vector2.Distance(myBrain.legs.getPosition(), sheepBrain.legs.getPosition());

        if (distance < 0f)
        {
            distance = distance * (-1); //distance can't be negative
        }

        if (distance <= 1f)
        {
            if (sheepBrain.memory.GetValue<float>("HP") >= 60f)
            {

                UnityEngine.Debug.Log("Distance of " + myBrain.getGameObject() + " and " + sheepBrain.getGameObject() + " is: " + distance);
                sheepMemory.SetValue("BeingEaten", true);

                result = "EATING";
                mainMachine.RequestStateTransition(eating.GetTarget());
            }
            else
            {
                result = "FAIL";
                mainMachine.RequestStateTransition(roam.GetTarget());
            }
        }
        else
        {
            if (sheepBrain.memory.GetValue<float>("HP") < 60f)
            {
                result = "FAIL";
                mainMachine.RequestStateTransition(roam.GetTarget());
            }
        }

        //delete Beaconinfo after using
        curBeacon = null;

        yield return null;
    }
	
	void UpdateCaution(Transform player) {
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		//Debug.Log (myBrain.legs.getPosition());
		//Debug.Log(playerPos);
		
		//Debug.DrawLine(playerPos.ToWorldCoords(), playerPos.ToWorldCoords() + positionOffset.ToWorldCoords());
		//Debug.Log (Vector2.Dot(playerFacing.normalized, positionOffset.normalized));
		
		if(Vector2.Dot(playerFacing.normalized, positionOffset.normalized) >= 0.71f) {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") + (Time.deltaTime * (1 / positionOffset.magnitude)));
		} else {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
		}
		if(myBrain.memory.GetValue<float>("watched") > (1 / myBrain.memory.GetValue<float>("caution")) * fleeThreshold) {
			myBrain.memory.SetValue ("shouldHide", 2f);
		}
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
        retV.Add(new LinkedStateReference(eating, "Eating"));
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

    public void createARFF(string sheep, string result)
    {
        string curFile = @"./trainedData.arff";
        StreamWriter fileWriter;

        if (File.Exists(curFile))
        {
            fileWriter = new StreamWriter(curFile, true);
        }
        else
        {
            fileWriter = new StreamWriter(curFile);
            fileWriter.WriteLine("@relation RATE");
            fileWriter.WriteLine("@attribute PANIC numeric");
            fileWriter.WriteLine("@attribute COURAGE numeric");
            fileWriter.WriteLine("@attribute CHASEDBY numeric");
            fileWriter.WriteLine("@attribute distanceWithMe numeric");
            fileWriter.WriteLine("@attribute distanceWithSheperd numeric");
            fileWriter.WriteLine("@attribute myHungryLevel numeric");
            fileWriter.WriteLine("@attribute sheepHP numeric");
            fileWriter.WriteLine("@attribute class {SUCCESS,FAIL}");
            fileWriter.WriteLine("@data");
        }

        sheep = sheep + "," + result;

        fileWriter.WriteLine(sheep);
        fileWriter.Close();
    }
}

