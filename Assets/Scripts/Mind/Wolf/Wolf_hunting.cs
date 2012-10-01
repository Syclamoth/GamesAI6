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
    private Flee fleeBehaviour;

    private float ferocityRate;
    private Vector2 oldTargetPosition;
    
    bool stillSeeTarget;
    private float distanceWithMe;
    private float distanceWithSheperd;
    private float hungry;
    private float sheepHP;
    private string result;

    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
		Legs myLeg = myBrain.legs;
        arriveBehaviour = new PathfindToPoint();
        seekBehaviour = new Seek();
        fleeBehaviour = new Flee();

		arriveBehaviour.Init(myLeg, myBrain.levelGrid);
        seekBehaviour.Init(myLeg);
        fleeBehaviour.Init(myLeg);

		myLeg.addSteeringBehaviour(arriveBehaviour);
        myLeg.addSteeringBehaviour(seekBehaviour);
        myLeg.addSteeringBehaviour(fleeBehaviour);

        myLeg.maxSpeed = 11f;
        time = 0f;
        ferocityRate = controller.memory.GetValue<float>("ferocity");

        sheepTarget = myBrain.memory.GetValue<SensedObject>("hasCommand");
        sheepMemory = sheepTarget.getMemory();
        sheepBrain = (Brain)sheepTarget.getObject().GetComponent("Brain");

        fleeBehaviour.setTarget(GameObject.FindGameObjectWithTag("Player"));
        arriveBehaviour.setTarget(sheepTarget.getObject());

        distanceWithMe = Vector2.Distance(sheepBrain.legs.getPosition(), myBrain.legs.getPosition());
        distanceWithSheperd = Vector2.Distance(sheepBrain.legs.getPosition(), ((Legs)GameObject.FindGameObjectWithTag("Player").GetComponent<Legs>()).getPosition());
        
        hungry = myBrain.memory.GetValue<float>("hungryLevel");
        sheepHP = sheepBrain.memory.GetValue<float>("HP");

        yield return null;
    }
    public override IEnumerator Exit()
    {
		myBrain.legs.removeSteeringBehaviour(arriveBehaviour);
        myBrain.legs.removeSteeringBehaviour(seekBehaviour);
        myBrain.legs.removeSteeringBehaviour(fleeBehaviour);

        createARFF(sheepBrain, hungry, distanceWithMe, distanceWithSheperd, sheepHP, result);
        if (result == "SUCCESS")
        {
            myBrain.memory.SetValue("hungryLevel", myBrain.memory.GetValue<float>("hungryLevel") + 4f);
        }

        yield return null;
    }
    public override IEnumerator Run(Brain controller)
    {
        List<GameObject> seenSheep = new List<GameObject>();

        bool thereIsShepherd = false;

        foreach (SensedObject obj in controller.senses.GetSensedObjects())
        {
            if(obj.getAgentType().Equals(AgentClassification.Sheep))
            {
                seenSheep.Add(obj.getObject());
            }
            if(obj.getAgentType().Equals(AgentClassification.Shepherd))
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

        if (thereIsShepherd)
        {
            fleeBehaviour.setWeight(fleeBehaviour.getWeight() + (Time.deltaTime / myBrain.memory.GetValue<float>("ferocity")));
            if (fleeBehaviour.getWeight() > 15f)
            {
                fleeBehaviour.setWeight(15f);
            }
        }
        else
        {
            fleeBehaviour.setWeight(0f);

            if (stillSeeTarget)
            {
                //increase weight based on ferocityRate and rateShepherd. If the wolf see the Sheperd, the thought to chase the sheep will be suppressed
                arriveBehaviour.setWeight(arriveBehaviour.getWeight() + (Time.deltaTime * ferocityRate));
                seekBehaviour.setWeight(0f);

                if (arriveBehaviour.getWeight() > 30f)
                {
                    arriveBehaviour.setWeight(30f);
                }
                oldTargetPosition = arriveBehaviour.getTarget();
                time = 0f;
            }
            else
            {
                time += Time.deltaTime;
                if (time > 3f)
                {
                    //keep chasing
                    seekBehaviour.setTarget(oldTargetPosition);
                    seekBehaviour.setWeight(seekBehaviour.getWeight() + (Time.deltaTime * ferocityRate));

                    //arrive's weight decreases
                    arriveBehaviour.setWeight(arriveBehaviour.getWeight() - (Time.deltaTime / ferocityRate));

                    if (seekBehaviour.getWeight() > 15f)
                    {
                        seekBehaviour.setWeight(15f);
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

            if (distance <= 2f)
            {
                if (sheepBrain.memory.GetValue<float>("HP") >= 60f)
                {

                    UnityEngine.Debug.Log("Distance of " + myBrain.getGameObject() + " and " + sheepBrain.getGameObject() + " is: " + distance);
                    sheepMemory.SetValue("BeingEaten", true);

                    result = "SUCCESS";
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

    public void createARFF(Brain sheepBrain, float hungry, float distanceWithMe, float distanceWithSheperd, float sheepHP, string result)
    {
        string curFile = @"./trainedData.arff";
        StreamWriter fileWriter;
        float panic = 0f;
        float courage = 0f;
        float chasedBy = 0f;
        string sheep = "";

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

        panic = sheepBrain.memory.GetValue<float>("Panic");
        courage = sheepBrain.memory.GetValue<float>("cowardLevel");
        chasedBy = sheepBrain.memory.GetValue<List<Brain>>("chasedBy").Count;

        sheep = panic + "," + courage + "," + chasedBy + "," + distanceWithMe + "," + distanceWithSheperd + "," + hungry + "," + sheepHP + "," + result;

        fileWriter.WriteLine(sheep);
        fileWriter.Close();
    }
}