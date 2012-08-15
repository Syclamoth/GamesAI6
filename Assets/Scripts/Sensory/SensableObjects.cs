using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SensableObjects : MonoBehaviour {
	
	public bool usingQuadtree = false;
	private HashSet<GameObject> objects = new HashSet<GameObject>();
	
	private QuadTree objectTree;
	
	void Update() {
		if(usingQuadtree) {
			objectTree = RebuildQuadTree(20, 8);
			//usingQuadtree = false;
			objectTree.DrawTree(Color.green);
		}
	}
	
	private QuadTree RebuildQuadTree(float treeScale, int maxDepth) {
		QuadTree tree = new QuadTree(treeScale, maxDepth);
		
		foreach(GameObject obj in objects) {
			tree.AddElement(new QuadtreeEntry(obj, new Vector2(obj.transform.position.x, obj.transform.position.z)));
		}
		return tree;
	}
	
	public List<GameObject> GetObjectsInRadius(Vector3 position, float radius)
	{
		// Currently implemented in the most naive fashion. Will add additional algorithms later,
		// with logic to choose the most efficient one for the current task.
		List<GameObject> retV = new List<GameObject>();
		float sqrRadius = radius * radius;
		foreach(GameObject obj in objects)
		{
			if((obj.transform.position - position).sqrMagnitude < sqrRadius)
			{
				retV.Add (obj);
			}
		}
		return retV;
	}
	
	public void RegisterObject(GameObject obj)
	{
		if(!objects.Add (obj))
		{
			Debug.Log ("Object " + obj.name + " already registered!");
		}
	}
	
	public void OnGUI() {
		for(int i = -5; i < 8; ++i) {
			GUI.color = QuadTree.GetSpectrum(i);
			GUILayout.Label("Depth: " + i);
		}
	}
}
