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
		
		if(Input.GetMouseButtonDown(0)){
			//Failed mouse input
		    Vector3 mousePositionInWorld = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mousePositionInWorld.y = 0;
			GameObject.Find("SheepTarget").transform.position = mousePositionInWorld;
		}
		
	}
}
