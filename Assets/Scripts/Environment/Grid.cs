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
		
	    grid = new GridSquare[width,height];
		
		//Loop through the array to create the actual grid O(n)
		for (j=0;j<height;++j) {
			for (i=0;i<width;++i) {
				grid[i,j] = new GridSquare(new Vector2(i,j),this);
				if ((i % 4 == 0) && (j % 4 == 0)) {
					GameObject instance = (GameObject) Instantiate(cubePrefab);
					
					instance.renderer.transform.position = new Vector3(x1+(i+0.5f)*(dWidth/width),y+0.5f,z1+(j+0.5f)*(dHeight/height));
					instance.renderer.transform.localScale = new Vector3(3*dWidth/width,1.0f,3*dHeight/height);
					boxManager.AddBox (instance.collider.bounds);
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
