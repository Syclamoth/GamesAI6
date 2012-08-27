using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public int width = 2;
    public int height = 2;
	public GameObject cubePrefab;
	public BoxManager boxManager;
	
	// Use this for initialization
    private GridSquare[,] grid;
	void Start () {
		uint i,j;
		
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
		
		//Loop through the array to create the actual grid O(n)
		for (j=0;j<height;++j) {
			for (i=0;i<width;++i) {
				grid[i,j] = new GridSquare(new Vector2(i,j),this);
				if ((i % 4 == 0) && (j % 4 == 0)) {
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
	}
	
	public GridSquare getGridSquare(int x, int y)
	{
		return grid[x,y];
	}
}

public class GridSquare {
    public Vector2 Position  {get; private set;}
	private Grid parent;
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
			if (Position.x >= parent.width)
				return null;
			return parent.getGridSquare((int)Position.x+1,(int)Position.y);
		}
	}
	
	public GridSquare(Vector2 position, Grid parent) {
		if (position == null || parent == null)
			throw new UnassignedReferenceException();
		
		Position = position;
		this.parent = parent;
	}
	
	
}
