using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public int width  = 2;
    public int height = 2;
	public int mazeLength = 20;
	public int numberOfBranches = 5;
	public GameObject cubePrefab;
	public GameObject debrisPrefab;
	public GameObject streetlampPrefab;
	
	public GameObject playerObject;
	public GameObject sheepSpawner;
	
	public BoxManager boxManager;
	
	// Use this for initialization
    private GridSquare[,] grid;
	private LinkedList<GridSquare> maze;
	private LinkedList<LinkedList<GridSquare>> mazeBranches;
	
	public System.Random rnd = new System.Random();
	
	void Start () {
		uint i,j;
		
		width = Mathf.Max (Mathf.CeilToInt((float)width/4.0f),2)*4-1;
		height = Mathf.Max (Mathf.CeilToInt((float)height/4.0f),2)*4-1;
		Vector3 topLeft = this.collider.bounds.center - this.collider.bounds.extents;
		Vector3 bottomRight= this.collider.bounds.center + this.collider.bounds.extents;
		float y = this.collider.bounds.center.y;
		
		float x1 = topLeft.x;
		float dWidth = bottomRight.x-x1;
		float z1 = topLeft.z;
		float dHeight = bottomRight.z-z1;
		
		bool[,] usedGrid = new bool[3,3];
		
	    grid = new GridSquare[width,height];
		
		float blockX,blockY,blockWidth,blockHeight;
		
		int addedHeight;
		
		int bX,bY;
		
		
		int startX = rnd.Next (1,width/4);
		int startY = rnd.Next (1,height/4);
		int endX=0,endY=0;
		
		GridSquare current,safeEntrance = null;
		
		
		//Loop through the array to create the actual grid O(n)
		for (j=0;j<height;++j) {
			for (i=0;i<width;++i) {
				grid[i,j] = new GridSquare(new Vector2(i,j),this);
			}
		}
		
		maze = null;
		while (maze == null)
		{
			startX = rnd.Next (1,width/4);
			startY = rnd.Next (1,height/4);
			maze = generateMaze(startX,startY);
		}
		
		LinkedListNode<GridSquare> node = maze.Last;
		
		while (node != null) {
			current = node.Value.getAdjacentBlock(startX,startY);
			if (current == null) {
				node = node.Previous;
				continue;
			}
			
			safeEntrance = node.Value;
			endX = (int)current.Position.x/4;
			endY = (int)current.Position.y/4;
			break;
		}
		
		for (j=0;j<height;++j) {
			for (i=0;i<width;++i) {
				if ((i % 4 == 0) && (j % 4 == 0)) {
					
					//Sidewalk
					GameObject instance = (GameObject) Instantiate(cubePrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+0.1f,z1+(j+1.5f)*(dHeight/height));
					instance.transform.localScale = new Vector3(3*dWidth/width,0.2f,3*dHeight/height);
					instance.transform.parent = this.transform;
					instance.collider.enabled = false;
					if (endX*4 == i && endY*4 == j) {
						instantiateEnclave(endX,endY,safeEntrance);
						continue;
					}
					if (startX*4 == i && startY*4 == j) {
						//Starting block
						current = grid[startX*4+1,startY*4+1];
						
						instantiateEnclave(startX,startY,maze.First.Value);
						
						playerObject.transform.position = current.toVector3 ();
						sheepSpawner.transform.position = current.toVector3();
						sheepSpawner.GetComponent<SheepSpawn>().SpawnSheep();
						
						/*if (current.Bottom.Bottom != maze.First.Value) {
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
							instance.transform.localScale = new Vector3(2.6f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						else
						{
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+0.6f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
							
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+2.4f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						
						if (current.Top.Top != maze.First.Value) {
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
							instance.transform.localScale = new Vector3(2.6f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						else
						{
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+0.6f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
							
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+2.4f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						
						if (current.Left.Left != maze.First.Value) {
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+1.5f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,2.6f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						else
						{
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+0.6f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
							
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+2.4f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						
						if (current.Right.Right != maze.First.Value) {
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+1.5f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,2.6f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}
						else
						{
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+0.6f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
							
							instance = (GameObject) Instantiate(cubePrefab);
							instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+2.4f)*(dHeight/height));
							instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
							instance.transform.parent = this.transform;
							boxManager.AddBox(instance.collider.bounds);
						}*/
						continue;
					}
					
					//Streelamps
					instance = (GameObject) Instantiate(streetlampPrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width)-1.4f*dWidth/width,y+0.1f,z1+(j+1.5f)*(dHeight/height)-1.4f*dHeight/height);
					instance.transform.parent = this.transform;
					instance.transform.Rotate(new Vector3(0,-135.0f,0));
					
					instance = (GameObject) Instantiate(streetlampPrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width)+1.4f*dWidth/width,y+0.1f,z1+(j+1.5f)*(dHeight/height)-1.4f*dHeight/height);
					instance.transform.parent = this.transform;
					instance.transform.Rotate(new Vector3(0,135.0f,0));
					
					instance = (GameObject) Instantiate(streetlampPrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width)-1.4f*dWidth/width,y+0.1f,z1+(j+1.5f)*(dHeight/height)+1.4f*dHeight/height);
					instance.transform.parent = this.transform;
					instance.transform.Rotate(new Vector3(0,-45.0f,0));
					
					instance = (GameObject) Instantiate(streetlampPrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width)+1.4f*dWidth/width,y+0.1f,z1+(j+1.5f)*(dHeight/height)+1.4f*dHeight/height);
					instance.transform.parent = this.transform;
					instance.transform.Rotate(new Vector3(0,+45.0f,0));
					
					//Collision Box
					instance = (GameObject) Instantiate(cubePrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+5.0f,z1+(j+1.5f)*(dHeight/height));
					instance.transform.localScale = new Vector3(2.5f*dWidth/width,10.0f,2.5f*dHeight/height);
					instance.renderer.enabled = false;
					instance.transform.parent = this.transform;
					boxManager.AddBox (instance.collider.bounds);
					
					
					blockX = (x1+(i+1.5f)*(dWidth/width)) - (0.5f*2.5f*dWidth/width);
					blockY = (z1+(j+1.5f)*(dHeight/height)) - (0.5f*2.5f*dHeight/height);
					blockWidth  = 2.5f*dWidth/width;
					blockHeight = 2.5f*dHeight/height;
					
					//Building Styles
					switch(rnd.Next (0,3)) {
					case 0:
						addedHeight = rnd.Next (-6,7);
						if (addedHeight >= 4)
						{
							addedHeight += rnd.Next (50,100);
						}
						instance = (GameObject) Instantiate(cubePrefab);
						instance.transform.position = new Vector3(blockX + 0.5f * blockWidth,y+15.0f+addedHeight*0.5f,blockY + 0.5f * blockHeight);
						instance.transform.localScale = new Vector3(blockWidth,30.0f+addedHeight,blockHeight);
						instance.transform.parent = this.transform;
						instance.collider.enabled = false;
						break;
					case 1:
						for (bX = 0;bX < 3;++bX) {
							for (bY = 0; bY < 3; ++bY) {
								usedGrid[bX,bY] = false;
							}
						}
						
						bX = rnd.Next (0,2);
						bY = rnd.Next (0,2);
						
						instance = (GameObject) Instantiate(cubePrefab);
						addedHeight = rnd.Next (1,8);
						instance.transform.position = new Vector3(blockX + bX * (blockWidth/3.0f) + (blockWidth/3),y+5.0f+addedHeight*0.5f,blockY + bY * (blockHeight/3.0f)+ (blockHeight/3));
						instance.transform.localScale = new Vector3(1.9f*(blockWidth/3),10.0f+addedHeight,1.9f*(blockHeight/3));
						instance.transform.parent = this.transform;
						instance.collider.enabled = false;
						
						usedGrid[bX,bY] = true;
						usedGrid[bX+1,bY] = true;
						usedGrid[bX,bY+1] = true;
						usedGrid[bX+1,bY+1] = true;
						
						for (bX = 0;bX < 3;++bX) {
							for (bY = 0; bY < 3; ++bY) {
								if (usedGrid[bX,bY] == false)
								{
									instance = (GameObject) Instantiate(cubePrefab);
									addedHeight = rnd.Next (2,6);
									instance.transform.position = new Vector3(blockX + bX * (blockWidth/3.0f) + 0.5f * (blockWidth/3),y+2.5f+addedHeight*0.5f,blockY + bY * (blockHeight/3.0f)+ 0.5f * (blockHeight/3));
									instance.transform.localScale = new Vector3(0.9f*(blockWidth/3),5.0f+addedHeight,0.9f*(blockHeight/3));
									instance.transform.parent = this.transform;
									instance.collider.enabled = false;
								}
							}
						}
						break;
					default:
						for (bX = 0;bX < 3;++bX) {
							for (bY = 0; bY < 3; ++bY) {
								instance = (GameObject) Instantiate(cubePrefab);
								addedHeight = rnd.Next (2,6);
								instance.transform.position = new Vector3(blockX + bX * (blockWidth/3.0f) + 0.5f * (blockWidth/3),y+2.5f+addedHeight*0.5f,blockY + bY * (blockHeight/3.0f)+ 0.5f * (blockHeight/3));
								instance.transform.localScale = new Vector3(0.9f*(blockWidth/3),5.0f+addedHeight,0.9f*(blockHeight/3));
								instance.transform.parent = this.transform;
								instance.collider.enabled = false;
							}
						}
						break;
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 topLeft = this.collider.bounds.center - this.collider.bounds.extents;
		Vector3 bottomRight= this.collider.bounds.center + this.collider.bounds.extents;
		float y = this.collider.bounds.center.y;
		
		float x1 = topLeft.x;
		float dWidth = bottomRight.x-x1;
		float z1 = topLeft.z;
		float dHeight = bottomRight.z-z1;
		
		uint i;
		Vector3 lineFrom,lineTo;
		for (i=0;i<=width;++i)
		{
			lineFrom = new Vector3(x1 + ((float)i/width)*dWidth, y, z1);
			lineTo = new Vector3(x1 + ((float)i/width)*dWidth, y, z1 + dHeight);
			Debug.DrawLine (lineFrom,lineTo,Color.blue);
		}
		
		for (i=0;i<=height;++i)
		{
			lineFrom = new Vector3(x1, y, z1 + ((float)i/height)*dHeight);
			lineTo = new Vector3(x1 + dWidth, y, z1 + ((float)i/height)*dHeight);
			Debug.DrawLine (lineFrom,lineTo,Color.blue);
		}
		//Debug.DrawLine (topLeft,bottomRight,Color.blue);
		if (maze != null)
		{
			LinkedListNode<GridSquare> node = maze.First;
			if (node != null) {
				while (node.Next != null) {
					lineFrom = node.Value.toVector3 ();
					node = node.Next;
					lineTo = node.Value.toVector3 ();
					Debug.DrawLine (lineFrom,lineTo,Color.green);
				}
			}
			
			foreach (LinkedList<GridSquare> branch in mazeBranches)
			{
				node = branch.First;
				if (node != null) {
					while (node.Next != null) {
						lineFrom = node.Value.toVector3 ();
						node = node.Next;
						lineTo = node.Value.toVector3 ();
						Debug.DrawLine (lineFrom,lineTo,Color.yellow);
					}
				}
			}
		}
	}
	
	private void instantiateEnclave(int blockX, int blockY, GridSquare entrance) {
		//Starting block
		GridSquare current = grid[blockX*4+1,blockY*4+1];
		
		width = Mathf.Max (Mathf.CeilToInt((float)width/4.0f),2)*4-1;
		height = Mathf.Max (Mathf.CeilToInt((float)height/4.0f),2)*4-1;
		Vector3 topLeft = this.collider.bounds.center - this.collider.bounds.extents;
		Vector3 bottomRight= this.collider.bounds.center + this.collider.bounds.extents;
		float y = this.collider.bounds.center.y;
		
		float x1 = topLeft.x;
		float dWidth = bottomRight.x-x1;
		float z1 = topLeft.z;
		float dHeight = bottomRight.z-z1;
		int i = blockX*4,j = blockY*4;
		
		GameObject instance;
		
		if (current.Bottom.Bottom != entrance) {
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
			instance.transform.localScale = new Vector3(2.6f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		else
		{
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+0.6f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
			
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+2.4f)*(dWidth/width),y+2.5f,z1+(j+2.8f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		
		if (current.Top.Top != entrance) {
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
			instance.transform.localScale = new Vector3(2.6f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		else
		{
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+0.6f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
			
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+2.4f)*(dWidth/width),y+2.5f,z1+(j+0.2f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.8f*dWidth/width,5.0f,0.1f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		
		if (current.Left.Left != entrance) {
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+1.5f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,2.6f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		else
		{
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+0.6f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
			
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+0.2f)*(dWidth/width),y+2.5f,z1+(j+2.4f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		
		if (current.Right.Right != entrance) {
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+1.5f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,2.6f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
		else
		{
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+0.6f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
			
			instance = (GameObject) Instantiate(cubePrefab);
			instance.transform.position = new Vector3(x1+(i+2.8f)*(dWidth/width),y+2.5f,z1+(j+2.4f)*(dHeight/height));
			instance.transform.localScale = new Vector3(0.1f*dWidth/width,5.0f,0.8f*dHeight/height);
			instance.transform.parent = this.transform;
			boxManager.AddBox(instance.collider.bounds);
		}
	}
	
	public GridSquare getGridSquare(int x, int y)
	{
		return grid[x,y];
	}
	
	private LinkedList<GridSquare> generateMaze(int startX,int startY) {
				
		//Generate maze

		//Pick a starting gridsquare (around the starting block)
		GridSquare current, starting;
		bool[] visited = new bool[width*height];
		bool[] explored = new bool[width*height];
		uint left = (uint)mazeLength-1;
		LinkedList<GridSquare> maze = new LinkedList<GridSquare>();
		mazeBranches = new LinkedList<LinkedList<GridSquare>>();
		current = grid[startX*4,startY*4].Bottom.Right;
		switch (rnd.Next (0,4)) {
		case 0:
			starting = current.Left.Left;
			break;
		case 1:
			starting = current.Top.Top;
			break;
		case 2:
			starting = current.Right.Right;
			break;
		default:
			starting = current.Bottom.Bottom;
			break;
		}
		
		current = starting;
		visited[current.getHash ()] = true;
		explored[current.getHash ()] = true;
		maze.AddLast (new LinkedListNode<GridSquare>(current));
		GridSquare evaluating;
		List<GridSquare> neighbors = new List<GridSquare>();
		LinkedList<GridSquare> branches = new LinkedList<GridSquare>();
		List<GridSquare> junctions = new List<GridSquare>();
		
		while (left > 0) {
			neighbors = getGoodNeighbors(current,visited,true);
			
			if (neighbors.Count == 0)
			{
				if (branches.Count == 0)
				{
					Debug.Log ("No maze could be generated :(");
					return null;
				}
				
				current = branches.Last.Value;
				
				if (getGoodNeighbors(current,visited,true).Count <= 1)
					branches.RemoveLast ();
				
				while (maze.Last.Value != current) {
					visited[maze.Last.Value.getHash ()] = false;
					maze.RemoveLast ();
					left++;
					if (maze.Count == 0)
					{
						Debug.Log ("No maze could be generated :|");
						return null;
					}
				}
				
				continue;
			}
			
			if (neighbors.Count > 1) {
				branches.AddLast (new LinkedListNode<GridSquare>(current));
				junctions.Add (current);
			}
			evaluating = neighbors[rnd.Next (0,neighbors.Count)];
			if (current.isJunction())
			{
				current.Junction.exploreSquare(evaluating);
			}
			if (evaluating.isJunction())
			{
				evaluating.Junction.exploreSquare(current);
			}
			current = evaluating;
			visited[current.getHash ()] = true;
			explored[current.getHash ()] = true;
			maze.AddLast (new LinkedListNode<GridSquare>(current));
			--left;
			
			
		}
		if (junctions[junctions.Count - 1] != maze.Last.Value)
			junctions.Add (maze.Last.Value);
		
		/* Generate maze branches (TODO will refactor later)*/
		int branchesDone = 0;
		
		LinkedList<GridSquare> newBranch;
		GridSquare firstJunction;
		for (branchesDone = 0;branchesDone < numberOfBranches; branchesDone++)
		{
			newBranch = new LinkedList<GridSquare>();
			firstJunction = null;
			int tries = 50;
			while (true) {
				
				if (--tries == 0)
					break;
				if (getGoodNeighbors(firstJunction = junctions[rnd.Next (0,junctions.Count)],visited).Count == 0)
					continue;
				if (!visited[firstJunction.getHash ()])
					continue;
				break;
			}
			
			if (tries > 0)
			{
				current = firstJunction;
				
				newBranch.AddLast (new LinkedListNode<GridSquare>(current));
				while ((neighbors = getGoodNeighbors(current,visited)).Count > 0)
				{
					current = neighbors[rnd.Next (neighbors.Count)];
					if (current.isJunction ()) {
						junctions.Add (current);
					}
					visited[current.getHash ()] = true;
					explored[current.getHash ()] = true;
					newBranch.AddLast (new LinkedListNode<GridSquare>(current));
				}
				
				mazeBranches.AddLast (new LinkedListNode<LinkedList<GridSquare>>(newBranch));
			}
		}
		
		Debug.Log ("Maze generated...");
		
		foreach (GridSquare node in junctions)
		{
			GridSquare[] squares = {node.Top,node.Left,node.Bottom,node.Right};
		
			foreach (GridSquare s in squares)
			{
				if (s == null)
					continue;
				if (visited[s.getHash ()])
					continue;
				s.placeDebris();
			}
		}
		return maze;
	}
	
	private bool evaluateSquare(GridSquare square,GridSquare current,bool[] visited)
	{
		GridSquare[] squares = {square.Top,square.Left,square.Bottom,square.Right};
		
		foreach (GridSquare s in squares)
		{
			if (s == current)
				continue;
			if (s == null)
				return false;
			if (visited[s.getHash ()])
				return false;
		}
		return true;
	}
	
	private List<GridSquare> getGoodNeighbors(GridSquare current,bool[] visited)
	{
		List<GridSquare> neighbors = new List<GridSquare>();
		GridSquare[] squares = {current.Top,current.Left,current.Bottom,current.Right};
		GridSquare evaluating;
		
		foreach (GridSquare s in squares)
		{
			if (s == null)
				continue;
			if (visited[s.getHash ()])
				continue;
			if (s.isBlocked ())
				continue;
			if (evaluateSquare (s,current,visited))
				neighbors.Add (s);
		}
		
		return neighbors;
	}
	
	private List<GridSquare> getGoodNeighbors(GridSquare current,bool[] visited,bool onlyUnexplored)
	{
		List<GridSquare> neighbors = new List<GridSquare>();
		GridSquare[] squares = {current.Top,current.Left,current.Bottom,current.Right};
		GridSquare evaluating;
		
		foreach (GridSquare s in squares)
		{
			if (s == null)
				continue;
			if (visited[s.getHash ()])
				continue;
			if (s.isBlocked ())
				continue;
			if (onlyUnexplored && current.isJunction()) {
				if (current.Junction.hasExplored (s))
					continue;
			}
			if (evaluateSquare (s,current,visited))
				neighbors.Add (s);
		}
		
		return neighbors;
	}
}

public class GridSquare {
    public Vector2 Position  {get; private set;}
	private Grid parent;
	private bool blocked;
	private Junction junction = null;
	
    public GridSquare Top {
		get {
			if (Position.y <= 0.0f)
				return null;
			return parent.getGridSquare((int)Position.x,(int)Position.y-1);
		}
	}
	public GridSquare Bottom {
		get {
			if ((int)Position.y >= parent.height-1)
				return null;
			return parent.getGridSquare((int)Position.x,(int)Position.y+1);
		}
	}
	public GridSquare Left {
		get {
			if (Position.x <= 0.0f)
				return null;
			return parent.getGridSquare((int)Position.x-1,(int)Position.y);
		}
	}
	public GridSquare Right {
		get {
			if (Position.x >= parent.width - 1)
				return null;
			return parent.getGridSquare((int)Position.x+1,(int)Position.y);
		}
	}
	
	public GridSquare(Vector2 position, Grid parent) {
		if (position == null || parent == null)
			throw new UnassignedReferenceException();
		
		Position = position;
		this.parent = parent;
		blocked = false;
	}
	
	public bool isRoad() {
		if (Position.x % 4 == 3 || Position.y % 4 == 3)
			return true;
		return false;
	}
	
	public bool isJunction() {
		bool hor = false;
		GridSquare[] hSquares= {this.Left,this.Right};
		GridSquare[] vSquares= {this.Top,this.Bottom};
		foreach (GridSquare s in hSquares) {
			if (s == null)
				continue;
			if (s.isRoad ()) {
				hor = true;
				break;
			}
		}
		
		if (!hor)
			return false;
		
		foreach (GridSquare s in vSquares) {
			if (s == null)
				continue;
			if (s.isRoad ()) {
				return true;
			}
		}
		
		return false;
	}
	
	/* getAdjacentBlock()
	 * if and only if the square is on the road and infront of the middle of a
	 * block will this return a gridsquare.
	 * 
	 * Fig. 24601
	 * BBB| |BBB
	 * BBB|*|BBB
	 * BBB| |BBB
	 * 
	 * If there is more than one accessible block, one will be chosen at random
	 * Will never return the starting block
	 * Returns null if no match found
	 */
	public GridSquare getAdjacentBlock(int startX,int startY)
	{
		if (!isRoad ())
			return null;
		List<GridSquare> squares = new List<GridSquare>();
		
		if (Position.x % 4 == 1) {
			if (this.Top != null && !this.Top.isInStartBlock(startX,startY))
				squares.Add (this.Top.Top);
			if (this.Bottom != null && !this.Bottom.isInStartBlock(startX,startY))
				squares.Add (this.Bottom.Bottom);
		}
		else if (Position.y % 4 == 1) {
			if (this.Left != null && !this.Left.isInStartBlock(startX,startY))
				squares.Add (this.Left.Left);
			if (this.Right != null && !this.Right.isInStartBlock(startX,startY))
				squares.Add (this.Right.Right);
		}
		else {
			return null;
		}
		
		if (squares.Count == 0)
			return null;
		
		return (squares[parent.rnd.Next (squares.Count)]);
	}
	
	/* isInStartBlock()
	 * returns true if square is in the starting block. obviously.
	 */
	public bool isInStartBlock(int startX, int startY)
	{
		if (((int)Position.x)/4 == startX && ((int)Position.y)/4 == startY && !isRoad ())
			return true;
		return false;
	}
	
	public Junction Junction {
		get {
			if (!isJunction())
				return null;
			if (junction == null)
				junction = new Junction(this);
			return junction;
		}
	}
	
	public void block() {
		this.blocked = true;
	}
	public void unblock() {
		this.blocked = false;
	}
	
	public bool isBlocked() {
		if (!isRoad () || blocked)
			return true;
		return false;
	}
	
	public uint getHash() {
		return (uint)Position.y * (uint)parent.width + (uint)Position.x;
	}
					
	public Vector3 toVector3() {
		Vector3 topLeft = parent.collider.bounds.center - parent.collider.bounds.extents;
		Vector3 bottomRight= parent.collider.bounds.center + parent.collider.bounds.extents;
		float y = parent.collider.bounds.center.y;
		
		float x1 = topLeft.x;
		float dWidth = bottomRight.x-x1;
		float z1 = topLeft.z;
		float dHeight = bottomRight.z-z1;
		
		return new Vector3(x1 + ((this.Position.x+0.5f)/parent.width)*dWidth, y, 
			               z1 + ((this.Position.y+0.5f)/parent.height)*dHeight);
	}
	
	public float Width {
		get {
			Vector3 topLeft = parent.collider.bounds.center - parent.collider.bounds.extents;
			Vector3 bottomRight= parent.collider.bounds.center + parent.collider.bounds.extents;
			
			float x1 = topLeft.x;
			float dWidth = bottomRight.x-x1;
			
			return dWidth/parent.width;
		}
	}
	
	public float Length {
		get {
			Vector3 topLeft = parent.collider.bounds.center - parent.collider.bounds.extents;
			Vector3 bottomRight= parent.collider.bounds.center + parent.collider.bounds.extents;
			
			float z1 = topLeft.z;
			float dLength = bottomRight.z-z1;
			
			return dLength/parent.height;
		}
	}
	
	public void placeDebris() {
		if (isBlocked())
			return;
		
		
		GameObject instance = (GameObject) Object.Instantiate(parent.cubePrefab);
		
		instance.transform.position = this.toVector3();
		instance.transform.localScale = new Vector3(this.Width*1.5f,10.0f,this.Length*1.5f);
		instance.transform.parent = parent.transform;
		instance.renderer.enabled = false;
		parent.boxManager.AddBox (instance.collider.bounds);
		
		instance = (GameObject) Object.Instantiate(parent.debrisPrefab);
		
		instance.transform.position = this.toVector3();
		instance.transform.localScale = new Vector3(this.Width*1.4f,this.Width*1.4f,this.Width*1.4f);
		instance.transform.parent = parent.transform;
		instance.collider.enabled = false;
		
		blocked = true;
	}
}

public class Junction {
	public bool top = false, bottom = false, left = false, right = false;
	public GridSquare Square {get; private set;}
	
	public Junction(GridSquare square)
	{
		Square = square;
	}
				
	public bool hasExplored(GridSquare square) {
		if (square == Square.Top)
			return top;
		if (square == Square.Bottom)
			return bottom;
		if (square == Square.Left)
			return left;
		if (square == Square.Right)
			return right;
		return false;
	}
	
	public void exploreSquare(GridSquare square) {
		if (square == Square.Top)
		{
			top = true;
			return;
		}
		if (square == Square.Bottom)
		{
			bottom = true;
			return;
		}
		if (square == Square.Left)
		{
			left = true;
			return;
		}
		if (square == Square.Right)
		{
			right = true;
			return;
		}
	}
}
