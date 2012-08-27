using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {
    public PlayerLegs legs;
	public float speed = 5;
	
	public SensableObjects allObjects;
	public BoxManager boxes;
	public Transform lookAt;
	
	void Start() {
		allObjects.RegisterObject(new SensableObject(gameObject, AgentClassification.Shepherd));
	}
	
	
	void Update () {
	    Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		float transformDistance = Time.deltaTime * speed;
		Ray moveRay = new Ray(transform.position, direction.ToWorldCoords() * transformDistance);
		Vector3 normal;
		float distance;
		if(boxes.Raycast(moveRay, out distance, out normal)) {
			if(Vector3.Dot(direction.ToWorldCoords(), normal) <= 0.2f) {
				transformDistance = Mathf.Min (transformDistance, distance);
			}
		}
		
        legs.translate(direction * transformDistance);
		Vector3 lookDirection = lookAt.position - transform.position;
		lookDirection.y = 0;
		transform.rotation = Quaternion.LookRotation(lookDirection);
	}
}
