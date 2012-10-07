using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WolfController : MonoBehaviour {
	
	List<Genome> aliveWolves = new List<Genome>();
	
	public float baseMutationRate = 1;
	public GameObject wolfPrefab;
	
	public SensableObjects allObjects;
	public BoxManager boxes;
	
	// load this at startup!
	Genome loadedGenome;
	
	Genome bestGenome {
		get {
			if(aliveWolves.Count == 0) {
				return loadedGenome;
			}
			Genome bestGen = aliveWolves[0];
			
			foreach(Genome curGen in aliveWolves) {
				if(curGen.Fitness() > bestGen.Fitness()) {
					bestGen = curGen;
				}
			}
			return bestGen;
		}
	}
	
	void Awake() {
		// Add deserialsiation here!
		loadedGenome = generateDefaultGenome();
	}
	
	// RIGHT HERE IS WHERE YOU SHOULD ADD NEW GENES!
	Genome generateDefaultGenome() {
		Genome result = new Genome(this);
		result.addGene("Ferocity", new Chromosome(0.8, 0.1f));
		result.addGene("Cunning", new Chromosome(0.4, 0.5f));
		result.addGene("PackSense", new Chromosome(0.5, 0.8f));
		return result;
	}
	
	public void SpawnWolf(Vector2 spawnPoint) {
		GameObject newWolf = (GameObject) Instantiate(wolfPrefab);
				
		Vector3 wolfPos = spawnPoint.ToWorldCoords();
		wolfPos.y += 0.5f;
		newWolf.transform.position = wolfPos;
		newWolf.transform.parent = this.transform;
		Brain wolfBrain = newWolf.GetComponent<Brain>();
		Genome newGenome = new Genome(bestGenome, baseMutationRate, this);
		aliveWolves.Add(newGenome);
		wolfBrain.memory.SetValue(new MemoryEntry("Genome", newGenome));
		wolfBrain.memory.SetValue(new MemoryEntry("StartPoint", spawnPoint));
		wolfBrain.Init(boxes, allObjects);
	}
}
