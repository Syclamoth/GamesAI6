using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SensableObject {
	public GameObject obj;
	public AgentClassification classification;
	
	public SensableObject(GameObject newObj, AgentClassification newClass) {
		obj = newObj;
		classification = newClass;
	}
}

public class SensableObjects : MonoBehaviour {
	
	public bool usingQuadtree = false;
	private HashSet<SensableObject> objects = new HashSet<SensableObject>();
	
	private QuadTree<SensableObject> objectTree;
	
	void Awake() {
		objectTree = RebuildQuadTree(20, 8);
	}
	
	void Update() {
		if(usingQuadtree) {
			objectTree = RebuildQuadTree(20, 8);
			//usingQuadtree = false;
			objectTree.DrawTree(Color.green);
		}
	}
	
	private QuadTree<SensableObject> RebuildQuadTree(float treeScale, int maxDepth) {
		QuadTree<SensableObject> tree = new QuadTree<SensableObject>(treeScale, maxDepth);
		
		foreach(SensableObject obj in objects) {
			tree.AddElement(new QuadtreeEntry<SensableObject>(obj, new Vector2(obj.obj.transform.position.x, obj.obj.transform.position.z)));
		}
		return tree;
	}
	
	public List<SensableObject> GetObjectsInRadius(Vector3 position, float radius)
	{
		// Currently implemented in the most naive fashion. Will add additional algorithms later,
		// with logic to choose the most efficient one for the current task.
		/*
		List<SensableObject> retV = new List<SensableObject>();
		float sqrRadius = radius * radius;
		foreach(SensableObject obj in objects)
		{
			if((obj.obj.transform.position - position).sqrMagnitude < sqrRadius)
			{
				retV.Add (obj);
			}
		}
		return retV;
		*/
		
		// NOW USING QUADTREE!
		
		return objectTree.GetElementsInCircle(new Vector2(position.x, position.z), radius);
	}
	
	public void RegisterObject(SensableObject obj)
	{
		if(!objects.Add (obj))
		{
			Debug.Log ("Object " + obj.obj.name + " already registered!");
		}
	}
	/*
	public void OnGUI() {
		for(int i = -5; i < 8; ++i) {
			GUI.color = QuadTree.GetSpectrum(i);
			GUILayout.Label("Depth: " + i);
		}
	}*/
}
