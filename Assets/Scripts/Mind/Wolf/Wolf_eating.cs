using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Wolf_eating : State
{

    public ExplicitStateReference roam = new ExplicitStateReference(null);

    Machine mainMachine;
    Brain myBrain;
    Arrive arriveBehaviour;

    private SensedObject sheepTarget;
    private Memory sheepMemory;
    private Brain sheepBrain;
	
	private float watchedLevelDecay = 1;
	private float cautionLevelDecay = 0.1f;
	private float fleeThreshold = 3f;

    private BeaconInfo curBeacon = null;

    private string result = "FAIL";

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

        arriveBehaviour = new Arrive();

        arriveBehaviour.Init(myLeg);

        arriveBehaviour.setTarget(sheepTarget.getObject());

        //speed is zero
        myBrain.legs.maxSpeed = 0f;

        Debug.Log("I'm eating:" + sheepTarget.getObject() + " HP: " + sheepMemory.GetValue<float>("HP"));

        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        
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
			sheepMemory.SetValue("BeingEaten", false);
        }

        createARFF(myBrain.memory.GetValue<string>("targeting"), result);

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        bool thereIsShepherd = false;
		Transform playerTrans = null;
        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if (obj.getAgentType().Equals(AgentClassification.Shepherd))
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

		myBrain.memory.SetValue("caution", myBrain.memory.GetValue<float>("caution") - cautionLevelDecay * Time.deltaTime);
		if(thereIsShepherd) {
			UpdateCaution(playerTrans);
		} else {
			myBrain.memory.SetValue ("watched", myBrain.memory.GetValue<float>("watched") - watchedLevelDecay * Time.deltaTime);
		}
		
		myBrain.memory.SetValue("watched", Mathf.Clamp(myBrain.memory.GetValue<float>("watched"), 0, Mathf.Infinity));
        arriveBehaviour.setWeight(arriveBehaviour.getWeight() + Time.deltaTime * myBrain.memory.GetValue<float>("ferocity"));

        if (arriveBehaviour.getWeight() > 10f)
        {
            arriveBehaviour.setWeight(10f);
        }

        myBrain.legs.maxSpeed = 0f;
        sheepMemory.SetValue("HP", sheepMemory.GetValue<float>("HP") - (Time.deltaTime * myBrain.memory.GetValue<float>("damage")));
        //Debug.Log("Sheep's HP is: " +  sheepMemory.GetValue<float>("HP"));

        if (sheepMemory.GetValue<float>("HP") <= 0f)
        {
            sheepTarget.getObject().SetActiveRecursively(false);

	        Debug.Log("I ate the sheep");
            controller.memory.SetValue("hungryLevel", myBrain.memory.GetValue<float>("hungryLevel") + 10f);
            result = "SUCCESS";

            mainMachine.RequestStateTransition(roam.GetTarget());
        }

        //delete curBeacon
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
		
		if(Vector2.Dot(playerFacing.normalized, positionOffset.normalized) > 0.71f) {
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
            fileWriter.WriteLine("");
            fileWriter.WriteLine("@attribute PANIC numeric");
            fileWriter.WriteLine("@attribute COURAGE numeric");
            fileWriter.WriteLine("@attribute CHASEDBY numeric");
            fileWriter.WriteLine("@attribute distanceWithMe numeric");
            fileWriter.WriteLine("@attribute distanceWithSheperd numeric");
            fileWriter.WriteLine("@attribute myHungryLevel numeric");
            fileWriter.WriteLine("@attribute sheepHP numeric");
            fileWriter.WriteLine("@attribute class {SUCCESS,FAIL}");
            fileWriter.WriteLine("");
            fileWriter.WriteLine("@data");
        }

        sheep = sheep + "," + result;

        fileWriter.WriteLine(sheep);
        fileWriter.Close();
    }
}
