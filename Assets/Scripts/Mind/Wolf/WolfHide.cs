using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfHide : State {

    public ExplicitStateReference roaming = new ExplicitStateReference(null);

    private Seek runTo;
	
    Machine mainMachine;
    Brain myBrain;
	
	Transform player;
	
	float playerViewDistance = 2.7f;
	
	float hiddenTime;
	
    public override IEnumerator Enter(Machine owner, Brain controller)
    {
        mainMachine = owner;
        myBrain = controller;
        Legs myLeg = myBrain.legs;
        runTo = new Seek();
        runTo.Init(myLeg);
		
		player = GameObject.FindGameObjectWithTag("Player").transform;
		
		hiddenTime = 0;
		
		myBrain.memory.SetValue ("shouldHide", 0f);
		myBrain.legs.maxSpeed = 11f;
        myLeg.addSteeringBehaviour(runTo);
        yield return null;
    }
    public override IEnumerator Exit()
    {
        myBrain.legs.removeSteeringBehaviour(runTo);
		myBrain.memory.SetValue ("watched", 0f);
        yield return null;
    }
	
    public override IEnumerator Run(Brain controller)
    {
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		if(Vector2.Dot(playerFacing.normalized, positionOffset.normalized) > 0.71f) {
			// Get out of the player's vision
			EscapeFromView();
		} else {
			// Stay out of the player's vision (get behind them if possible)
			StayOutOfView ();
		}
		
		if(hiddenTime > 2) {
			mainMachine.RequestStateTransition(roaming.GetTarget());
		}
        yield return null;
    }
	
	void EscapeFromView() {
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 rightViewEdge = playerFacing + new Vector2(player.right.x, player.right.z);
		Vector2 leftViewEdge = playerFacing - new Vector2(player.right.x, player.right.z);
		
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		Vector2 rightPoint = Vector3.Project(positionOffset, rightViewEdge);
		Vector2 leftPoint = Vector3.Project (positionOffset, leftViewEdge);
		
		float rightDistance = Vector2.Distance(positionOffset, rightPoint);
		float leftDistance = Vector2.Distance(positionOffset, leftPoint);
		
		float fleeDistance = playerViewDistance - positionOffset.magnitude;
		
		
		if(playerViewDistance > positionOffset.magnitude && fleeDistance < rightDistance && fleeDistance < leftDistance) {
			Vector2 runToPoint = positionOffset.normalized * playerViewDistance * 1.5f;
			runTo.setTarget(runToPoint);
			runTo.setWeight(15f);
			hiddenTime -= Time.deltaTime * 2;
			return;
		}
		if(playerViewDistance < positionOffset.magnitude) {
			hiddenTime += Time.deltaTime;
		} else {
			hiddenTime -= Time.deltaTime * 2;
		}
		if(rightDistance < leftDistance) {
			Vector2 runVector = rightPoint - positionOffset;
			Vector2 runToPoint = playerPos + positionOffset + (runVector * 2);
			runTo.setTarget(runToPoint);
			runTo.setWeight(15f);
			return;
		} else {
			Vector2 runVector = leftPoint - positionOffset;
			Vector2 runToPoint = playerPos + positionOffset + (runVector * 2);
			runTo.setTarget(runToPoint);
			runTo.setWeight(15f);
			return;
		}
	}
	
	void StayOutOfView() {
		hiddenTime += Time.deltaTime;
		Vector2 playerPos = new Vector2(player.position.x, player.position.z);
		Vector2 playerFacing = new Vector2(player.forward.x, player.forward.z);
		Vector2 positionOffset = myBrain.legs.getPosition() - playerPos;
		
		Vector2 runToPoint = myBrain.legs.getPosition() - playerFacing;
		
		runTo.setTarget(runToPoint);
		runTo.setWeight(15f);
	}
	
    public override ObservedVariable[] GetExposedVariables()
    {
        return new ObservedVariable[] {
		};
    }


    override public List<LinkedStateReference> GetStateTransitions()
    {
        List<LinkedStateReference> retV = new List<LinkedStateReference>();
        retV.Add(new LinkedStateReference(roaming, "Hidden"));
        return retV;
    }


    //State Machine editor
    override public string GetNiceName()
    {
        return "Wolf Hiding";
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
