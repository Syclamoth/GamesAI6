using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding {

	void calculatePath(Vector2 pathFrom,Vector2 pathTo)
	{
		AStarNode targetNode = null; // = Quadtree.getNodeFromVector(pathTo);
		List<AStarNode> closedSet = new List<AStarNode>();
		//Descending priority queue
		PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>(false);
		
		AStarNode temp = null; // = Quadtree.getNodeFromVector(pathFrom);
		openSet.enqueueWithPriority (temp,temp.getFScore());
		
		for(;;)
		{
			AStarNode current = openSet.dequeue ();
			if (current == default(AStarNode)) //Didn't find a path :(
				return;
			closedSet.Add (current);
			if (current == targetNode) //Found a path! :)
				return;
			foreach (AStarNode node in current.getNeighbors())
			{
				if (closedSet.Contains (node) /* || !node.isWalkable */)
				{
					continue; //ignore this node...
				}
				if (!openSet.Contains (node))
				{
					temp = new AStarNode(null);
					temp.setParent(current);
					openSet.enqueue (temp,temp.getFScore());
				}
				else
				{
					if ((node.position - current.position).magnitude + current.gScore <
						(node.position - node.getParent ().position).magnitude + node.getParent ().position)
					{
						openSet.Remove (node);
						node.setParent (current);
						openSet.enqueueWithPriority(node,node.getFScore());
					}
				}
			}
		}
	}
	
	class AStarNode
	{
		private object nodeContent;
		private AStarNode parent;
		private List<AStarNode> neighbors;
		
		public float gScore = 0.0f;
		public float hScore = 0.0f;
		public Vector2 position = null;
		
		public AStarNode(object contents)
		{
			parent = null;
			nodeContent = contents;
			neighbors = new List<AStarNode>();
			
			//hScore = 
		}
		
		public object getParent() {
			return parent;
		}
		
		public void setParent(AStarNode newParent)
		{
			parent = newParent;
		}
		
		public float getFScore()
		{
			return gScore + hScore;
		}
		
		public void addNeighbor(AStarNode node)
		{
			neighbors.Add (node);
		}
		
		public List<AStarNode> getNeighbors() {
			return neighbors;
		}
	}
}
