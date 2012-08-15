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
	
	public int bucketSize = 4;
	
	private int maxDepth;
	private float scale;
	
	private Node baseNode;
	
	public QuadTree(float treeScale, int maxDepth) {
		bucketSize = Mathf.Clamp(bucketSize, 1, int.MaxValue);
		scale = treeScale;
		this.maxDepth = maxDepth;
		baseNode = new Node(this, 0, Vector2.zero);
	}
	
	public void AddElement(QuadtreeEntry entry) {
		//Debug.Log (baseNode.CanContainElement(entry));
		
		
		while(!baseNode.CanContainElement(entry)) {
			float maxAxisOffset = (1f / Mathf.Pow(2, baseNode.GetDepth())) * scale;
			Vector2 positionOffset = entry.GetPosition() - baseNode.GetCentre();
			if(positionOffset.x > maxAxisOffset) {
				// Expand right
				if(positionOffset.y < -maxAxisOffset) {
					// Expand right-down (positive x, negative z)
					Node newBase = new Node(this, baseNode.GetDepth() - 1, baseNode.GetCentre() + new Vector2(maxAxisOffset, -maxAxisOffset));
					newBase.OverrideNode(baseNode, 2);
					baseNode = newBase;
					continue;
				} else {
					// Expand right-up (positive x, positive z) -- This is the problem!
					Node newBase = new Node(this, baseNode.GetDepth() - 1, baseNode.GetCentre() + new Vector2(maxAxisOffset, maxAxisOffset));
					newBase.OverrideNode(baseNode, 0);
					baseNode = newBase;
					continue;
				}
			} else {
				// Expand left
				if(positionOffset.y < -maxAxisOffset) {
					// Expand left-down
					Node newBase = new Node(this, baseNode.GetDepth() - 1, baseNode.GetCentre() + new Vector2(-maxAxisOffset, -maxAxisOffset));
					newBase.OverrideNode(baseNode, 3);
					baseNode = newBase;
					continue;
				} else {
					// Expand left-up
					Node newBase = new Node(this, baseNode.GetDepth() - 1, baseNode.GetCentre() + new Vector2(-maxAxisOffset, maxAxisOffset));
					newBase.OverrideNode(baseNode, 1);
					baseNode = newBase;
					continue;
				}
			}
		}
		
		baseNode.AddEntry(entry);
	}
	
	public void DrawTree(Color drawColour) {
		Debug.Log(baseNode.GetDepth() + " " + baseNode.GetCentre());
		baseNode.DrawNode(drawColour);
	}
	
	
	interface TreeElement {
		TreeElement AddEntry(QuadtreeEntry entry);
		void DrawNode(Color drawColour);
	}
	
	class Leaf : TreeElement{
		private List<QuadtreeEntry> entries;
		private int bucketSize;
		private int leafIndex;
		
		private Node parent;
		
		public TreeElement AddEntry(QuadtreeEntry entry) {
			entries.Add (entry);
			if(entries.Count > bucketSize && parent.GetDepth() < parent.GetBaseTree().maxDepth) {
				return new Node(this);
			} else {
				return this;
			}
		}
		
		public Leaf(int bucketSize, Node parent, int index) {
			this.parent = parent;
			this.bucketSize = bucketSize;
			entries = new List<QuadtreeEntry>();
			leafIndex = index;
		}
		
		public int GetBucketSize() {
			return bucketSize;
		}
		
		public int getIndex() {
			return leafIndex;
		}
		
		public List<QuadtreeEntry> GetEntries() {
			return entries;
		}
		public Node GetParent() {
			return parent;
		}
		public void DrawNode(Color drawColour) {
			//foreach(QuadtreeEntry entry in entries) {
			//	Debug.DrawLine(
			//}
		}
	}
	
	class Node : TreeElement{
		
		private int depth;
		private Vector2 centre;
		
		private QuadTree treeBase;
		
		private TreeElement[] branches = new TreeElement[4];
		
		public Node(Leaf original) {
			Node parent = original.GetParent();
			treeBase = parent.treeBase;
			depth = parent.depth + 1;
			centre = parent.centre;
			float centreOffset = (1f / Mathf.Pow(2, depth)) * treeBase.scale;
			int leafIndex = original.getIndex();
			if(leafIndex % 2 == 0) {
				centre.x -= centreOffset;
			} else {
				centre.x += centreOffset;
			}
			if(leafIndex / 2 == 0) {
				centre.y -= centreOffset;
			} else {
				centre.y += centreOffset;
			}
			for(int i = 0; i < branches.Length; ++i) {
				branches[i] = new Leaf(treeBase.bucketSize, this, i);
			}
			foreach(QuadtreeEntry entry in original.GetEntries()) {
				AddEntry(entry);
			}
		}
		
		public TreeElement AddEntry(QuadtreeEntry entry) {
			if(!CanContainElement(entry)) {
				//Debug.Log("Badly positioned entry!");
			}
			Vector2 entryPos = entry.GetPosition();
			int branchIndex = 0;
			if(entryPos.x > centre.x) {
				branchIndex += 1;
			}
			if(entryPos.y > centre.y) {
				branchIndex += 2;
			}
			branches[branchIndex] = branches[branchIndex].AddEntry(entry);
			return this;
		}
		
		public Node(QuadTree tree, int startingDepth, Vector2 startCentre) {
			treeBase = tree;
			depth = startingDepth;
			centre = startCentre;
			for(int i = 0; i < branches.Length; ++i) {
				branches[i] = new Leaf(treeBase.bucketSize, this, i);
			}
		}
		
		public void OverrideNode(TreeElement newElement, int index) {
			branches[index] = newElement;
		}
		
		public Vector2 GetCentre() {
			return centre;
		}
		public int GetDepth() {
			return depth;
		}
		public bool CanContainElement(QuadtreeEntry element) {
			float maxOffset = (1f / Mathf.Pow(2, depth)) * treeBase.scale;
			return new Rect(centre.x - maxOffset, centre.y - maxOffset, maxOffset * 2, maxOffset * 2).Contains(element.GetPosition());
		}
		public QuadTree GetBaseTree() {
			return treeBase;
		}
		public void DrawNode(Color drawColour) {
			Vector3 worldCentre = new Vector3(centre.x, 0, centre.y);
			float maxOffset = (1f / Mathf.Pow(2, depth)) * treeBase.scale;
			Debug.DrawLine(worldCentre + Vector3.forward * maxOffset, worldCentre - Vector3.forward * maxOffset, GetSpectrum(depth));
			Debug.DrawLine(worldCentre + Vector3.right * maxOffset, worldCentre - Vector3.right * maxOffset, GetSpectrum(depth));
			foreach(TreeElement element in branches) {
				element.DrawNode(drawColour);
			}
		}
	}
	public static Color GetSpectrum(float point)
	{
		float r = Mathf.Clamp01(Mathf.Sin(point));
		float g = Mathf.Clamp01(Mathf.Cos(point + (0.66f * Mathf.PI)));
		float b = Mathf.Clamp01(Mathf.Sin(point + (1.33f * Mathf.PI)));
		return new Color(r, g, b, 1f);
	}
}
	