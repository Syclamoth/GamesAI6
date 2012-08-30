using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepSpawn : MonoBehaviour {
	
	public float spawnRadius;
	public int numberToSpawn;
	
	public GameObject sheepPrefab;
	
	public SensableObjects allObjects;
	public BoxManager boxes;
	
	//The level generator will handle this.
	/*void Awake () {
		SpawnSheep();
	}*/
	
	public void SpawnSheep() {
		
		List<string> names = NameList.GetRandomisedNameList();
		for(int i = 0; i < numberToSpawn; ++i) {
			Vector2 spawnOffset = Random.insideUnitCircle;
			Vector3 curSpawnPoint = transform.position + new Vector3(spawnOffset.x, 0, spawnOffset.y);
			GameObject newSheep = (GameObject)Instantiate(sheepPrefab, curSpawnPoint, Quaternion.identity);
			newSheep.name = names[i % names.Count];
			Brain sheepBrain = newSheep.GetComponent<Brain>();
			sheepBrain.Init(boxes, allObjects);
			newSheep.transform.parent = transform;
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}
