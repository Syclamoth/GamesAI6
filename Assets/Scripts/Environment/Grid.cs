using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public int width  = 2;
    public int height = 2;
	public int mazeLength = 20;
	public GameObject cubePrefab;
	public BoxManager boxManager;
	
	// Use this for initialization
    private GridSquare[,] grid;
	private LinkedList<GridSquare> maze;
	
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
		
		System.Random rnd = new System.Random();
		
		
		int startX = rnd.Next (1,width/4);
		int startY = rnd.Next (1,height/4);
		
		//Loop through the array to create the actual grid O(n)
		for (j=0;j<height;++j) {
			for (i=0;i<width;++i) {
				grid[i,j] = new GridSquare(new Vector2(i,j),this);
				if ((i % 4 == 0) && (j % 4 == 0)) {
					if (startX*4 == i && startY*4 == j) {
						//Starting block
						continue;
					}
					//Sidewalk
					GameObject instance = (GameObject) Instantiate(cubePrefab);
					instance.transform.position = new Vector3(x1+(i+1.5f)*(dWidth/width),y+0.1f,z1+(j+1.5f)*(dHeight/height));
					instance.transform.localScale = new Vector3(3*dWidth/width,0.2f,3*dHeight/height);
					instance.transform.parent = this.transform;
					instance.collider.enabled = false;
					
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
		
		maze = null;
		while (maze == null)
		{
			maze = generateMaze(startX,startY);
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
		uint left = (uint)mazeLength;
		System.Random rnd = new System.Random();
		LinkedList<GridSquare> maze = new LinkedList<GridSquare>();
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
		maze.AddLast (new LinkedListNode<GridSquare>(current));
		GridSquare evaluating;
		List<GridSquare> neighbors = new List<GridSquare>();
		LinkedList<GridSquare> branches = new LinkedList<GridSquare>();
		
		while (left > 0) {
			neighbors = getGoodNeighbors(current,visited);
			
			if (neighbors.Count == 0)
			{
				if (branches.Count == 0)
				{
					Debug.Log ("No maze could be generated :(");
					return null;
				}
				current = branches.Last.Value;
				
				if (getGoodNeighbors(current,visited).Count <= 1)
					branches.RemoveLast ();
				
				while (maze.Last.Value != current) {
					//TODO code which blocks off alternate path
					visited[current.getHash ()] = false;
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
			
			if (neighbors.Count > 1)
				branches.AddLast (new LinkedListNode<GridSquare>(current));
			current = neighbors[rnd.Next (0,neighbors.Count)];
			visited[current.getHash ()] = true;
			maze.AddLast (new LinkedListNode<GridSquare>(current));
			--left;
			
			
		}
		Debug.Log ("Maze generated...");
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
}

public class GridSquare {
    public Vector2 Position  {get; private set;}
	private Grid parent;
	private bool blocked;
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
}
