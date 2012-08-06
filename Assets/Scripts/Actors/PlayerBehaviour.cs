using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {
    public PlayerLegs legs;
	public float speed = 5;
	
	public SensableObjects allObjects;
	
	void Start() {
		allObjects.RegisterObject(gameObject);
	}
	
	
	void Update () {
	    Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        legs.translate(direction * Time.deltaTime * speed);
	}
}
