using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadtreeEntry {
	private GameObject obj;
	private Vector2 positionInTree;
	
	public QuadtreeEntry(GameObject newObj, Vector2 newPosition) {
		obj = newObj;
		positionInTree = newPosition;
	}
	
	public Vector2 GetPosition() {
		return positionInTree;
	}
	public GameObject GetObject() {
		return obj;
	}
}

public class QuadTree {
	
	class Leaf {
		private List<QuadtreeEntry> entries;
		
		public bool AddEntry(QuadtreeEntry entry) {
			
		}
	}
	
	class Node {
		private Leaf leaf;
		private bool full = false;
		private Node[] nodes = new Node[4];
		
		
		public void AddEntry() {
			
		}
	}
	
}
