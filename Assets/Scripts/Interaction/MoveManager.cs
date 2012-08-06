using UnityEngine;
using System.Collections;

public class MoveManager {
	// The transform in the Unity game world which will be manipulated by this movement manager
	Transform myTrans;
	// The angle of the object, in degrees
	float angle = 0;
	
	public MoveManager (Transform newTrans)
	{
		myTrans = newTrans;
		// resets rotation, for consistency. Need some way of 'de-resetting' it!
		myTrans.rotation = Quaternion.identity;
	}
	
	
	// By default will map to an x-y coordinate system, which sits on the x-z plane in the game world.
	// Will remain 2D until I have a good reason to change it (the existence of bridges, basically).
	
	// Internal functions
	Vector2 WorldToLocalPoint(Vector3 inPos)
	{
		return new Vector2(inPos.x, inPos.z);
	}
	Vector3 LocalToWorldPoint(Vector2 inPos)
	{
		return new Vector3(inPos.x, 0, inPos.y);
	}
	Quaternion AngleToRotation(float angle)
	{
		return Quaternion.AngleAxis(angle, Vector3.up);
	}
	
	// Main functions
	public Vector2 GetPosition()	
	{
		return WorldToLocalPoint(myTrans.position);
	}
	
	public float GetAngle()
	{
		return angle;
	}
	
	public void MoveToPosition(Vector2 newPos)
	{
		myTrans.position = LocalToWorldPoint(newPos);
	}

    public void Translate(Vector2 offset)
    {
        myTrans.position += LocalToWorldPoint(offset);
    }

    public void Rotate(float offset)
    {
        angle += offset;
		myTrans.rotation = AngleToRotation (angle);
    }
	
	public void SetAngle(float newAngle)	
	{
		angle = newAngle;
		myTrans.rotation = AngleToRotation (angle);
	}
}
