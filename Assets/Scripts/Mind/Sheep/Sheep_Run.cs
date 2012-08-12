using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep_Run : State<Sheep>
{
    private static readonly Sheep_Run instance = new Sheep_Run();

    //send back state instance contains Sheep_Roaming
    public static Sheep_Run Instance
    {
        get
        {
            return instance;
        }
    }

    //Enter roaming state.
    public override IEnumerator Enter(Sheep sheep)
    {
        Debug.Log("Enter running state...");
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Enter(sheep));
    }
    public override IEnumerator Exit(Sheep sheep)
    {
        Debug.Log("Leaving running state...");
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Exit(sheep));
    }

    // Run will be called from within an update loop, but can be used to delay execution.
    public override IEnumerator Run(Sheep sheep)
    {
        yield return StartCoroutine(sheep.getStateMachine().getCurrentState().Run(sheep));
    }
}
