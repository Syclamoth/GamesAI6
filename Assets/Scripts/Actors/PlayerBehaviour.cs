using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour, IHearable
{
	public PlayerLegs legs;
	public float speed = 5;
	public SensableObjects allObjects;
	public Transform lookAt;
	public float beaconRange = 5;
	public float beaconCooldown = 10;
	public LayerMask collidesWith;
	
	
	private Vector2 velocity = Vector2.zero;
	
	private float curCooldown = 0;
	
	void Start ()
	{
		allObjects.RegisterObject (new SensableObject (gameObject, AgentClassification.Shepherd), this);
	}
	
	void Update ()
	{
		if(curCooldown <= 0 && Input.GetMouseButtonDown(0)) {
			foreach(SensableObject obj in allObjects.GetObjectsInRadius(transform.position, beaconRange)) {
				obj.obj.SendMessage("ImplantMemory",
					new MemoryEntry("LastBeacon", new BeaconInfo(Time.time, legs.getPosition())),
					SendMessageOptions.DontRequireReceiver);
			}
			StartCoroutine(ResetBeacon());
		}
		
		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		Vector2 desiredVelocity = direction * speed;
		
		Vector3 startPosition = transform.position;
		
		velocity = Vector2.Lerp (velocity, desiredVelocity, Time.deltaTime * 5);
		
		float transformDistance = Time.deltaTime * velocity.magnitude;
		Vector3 lookDirection = lookAt.position - transform.position;
		lookDirection.y = 0;
		transform.rotation = Quaternion.LookRotation (lookDirection);
		RaycastHit hit;
		if (Physics.SphereCast (startPosition, 0.5f, velocity.ToWorldCoords (), out hit, transformDistance + 0.1f, collidesWith)) {
			Vector3 newWorldVelocity = Vector3.Reflect (velocity.ToWorldCoords (), hit.normal);
			velocity = new Vector2 (newWorldVelocity.x, newWorldVelocity.z);
		}
		
		legs.translate (velocity * Time.deltaTime);
	}
	
	IEnumerator ResetBeacon() {
		curCooldown = beaconCooldown;
		while(curCooldown > 0) {
			curCooldown -= Time.deltaTime;
			yield return null;
		}
		curCooldown = 0;
	}
	
	public Volume getVolume() {
		return Volume.fromDecibels (50.0);
	}
	
	public GameObject getGameObject() {
		return gameObject;
	}
	public Vector2 getLocation() {
		return new Vector2(transform.position.x, transform.position.z);
	}
	public AgentClassification getClassification() {
		return AgentClassification.Shepherd;
	}
}

public class BeaconInfo {
	private float timeStamp;
	private Vector2 position;
	
	public BeaconInfo(float curTime, Vector2 position) {
		this.position = position;
		this.timeStamp = curTime;
	}
	
	public float GetTime() {
		return timeStamp;
	}
	public Vector2 GetPos() {
		return position;
	}
}