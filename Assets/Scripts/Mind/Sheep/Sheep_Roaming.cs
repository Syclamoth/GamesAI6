using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_Roaming : State<Sheep>
{
    private static readonly Sheep_Roaming instance = new Sheep_Roaming();

    //send back state instance contains Sheep_Roaming
    public static Sheep_Roaming Instance
    {
        get
        {
            return instance;
        }
    }

    //Enter roaming state.
    public override IEnumerator Enter(Sheep sheep)
    {
        Debug.Log("Enter roaming state...");
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Enter(sheep));
    }
    public override IEnumerator Exit(Sheep sheep)
    {
        Debug.Log("Leaving roaming state...");
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Exit(sheep));
    }

    // Run will be called from within an update loop, but can be used to delay execution.
    public override IEnumerator Run(Sheep sheep)
    {
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Run(sheep));
    }
}
