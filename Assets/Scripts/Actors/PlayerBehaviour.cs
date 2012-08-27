using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {
    public PlayerLegs legs;
	public float speed = 5;
	
	public SensableObjects allObjects;
	public Transform lookAt;
	
	public LayerMask collidesWith;
	
	private Vector2 velocity = Vector2.zero;
	
	void Start() {
		allObjects.RegisterObject(new SensableObject(gameObject, AgentClassification.Shepherd));
	}
	
	
	void Update () {
	    Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		Vector2 desiredVelocity = direction * speed;
		
		Vector3 startPosition = transform.position;
		
		velocity = Vector2.Lerp(velocity, desiredVelocity, Time.deltaTime * 5);
		
		float transformDistance = Time.deltaTime * velocity.magnitude;
		Vector3 lookDirection = lookAt.position - transform.position;
		lookDirection.y = 0;
		transform.rotation = Quaternion.LookRotation(lookDirection);
		RaycastHit hit;
		if(Physics.SphereCast(startPosition, 0.5f, velocity.ToWorldCoords(), out hit, transformDistance + 0.1f, collidesWith)) {
			Vector3 newWorldVelocity = Vector3.Reflect(velocity.ToWorldCoords(), hit.normal);
			velocity = new Vector2(newWorldVelocity.x, newWorldVelocity.z);
		}
		
        legs.translate(velocity * Time.deltaTime);
	}
}
