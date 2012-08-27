using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {
    public PlayerLegs legs;
	public float speed = 5;
	
	public SensableObjects allObjects;
	
	public Transform lookAt;
	
	void Start() {
		allObjects.RegisterObject(new SensableObject(gameObject, AgentClassification.Shepherd));
	}
	
	
	void Update () {
	    Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        legs.translate(direction * Time.deltaTime * speed);
		Vector3 lookDirection = lookAt.position - transform.position;
		lookDirection.y = 0;
		transform.rotation = Quaternion.LookRotation(lookDirection);
	}
}
