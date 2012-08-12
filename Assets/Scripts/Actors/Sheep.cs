using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep : MonoBehaviour{
    private Machine<Sheep> FSMSheep;

    //the higher, the slower the sheep is to react to danger. It will become crazy, w.e
    private int panicLevel;

    //the courage level of the sheep. The higher it is, the lesser the sheep's panic level increase.
    private int courageLevel;

    //every entity has a unique identifying number
    private int s_numID; //number
    private string s_ID; //with string. Ex: sheep_1

    //this is the next valid ID. Each time a BaseGameEntity is instantiated
    //this value is updated
    private int s_iNextValidID;

    public SensoryCortex senses;
    public List<State<Sheep>> controlledStates;
    

    public Sheep(int id)
    { 
        SetID(id);
    } 

    //this is called within the constructor to make sure the ID is set
    //correctly. It verifies that the value passed to the method is greater
    //or equal to the next valid ID, before setting the ID and incrementing
    //the next valid ID
    void SetID(int val)
    {
        if (val >= this.s_iNextValidID)
        {
            this.s_numID = val;
            this.s_iNextValidID = val++;
            this.s_ID = "sheep_" + s_numID;
        }
        else
        {
            Debug.LogError("SheepID is not set correctly! Disabling.");
            gameObject.active = false;
        }
    }

    public string getObjectID()
    {
        return this.s_ID;
    }

    public Machine<Sheep> getStateMachine()
    {
        return this.FSMSheep;
    }

    public void Awake()
    {
        Debug.Log("Sheep awakes...");
        FSMSheep = new Machine<Sheep>();
        FSMSheep.Configure(this, Sheep_Roaming.Instance);
    }

    public void ChangeState(State<Sheep> e)
    {
        FSMSheep.ChangeState(e);
    }

    public void IncreasePanicLevel()
    {
    }

    public void followSheperd()
    {
    }

    public void runAway()
    {

    }

    public void runRandomly()
    {

    }

    public void haveASeizure()
    {
    }

	// Use this for initialization
	void Start () {
        //Awake();
	}
	
	// Update is called once per frame.
	void Update () {
        IncreasePanicLevel();
        FSMSheep.Update();
	}

    void RunDebug()
    {
        foreach (SensedObject obj in senses.GetSensedObjects())
        {
            Debug.DrawLine(transform.position, obj.getObject().transform.position, Color.blue);
        }
    }
}
