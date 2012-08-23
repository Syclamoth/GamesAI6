using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public uint width;
    public uint height;
	// Use this for initialization
    private GridSquare[,] grid;
	void Start () {
	    grid = new GridSquare[width,height];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class GridSquare {
    public Vector2 Position  {get; private set;}
    public GridSquare Top    {get; private set;}
	public GridSquare Bottom {get; private set;}
	public GridSquare Left   {get; private set;}
	public GridSquare Right  {get; private set;}
	
	public GridSquare(Vector2 position)
	{
		
	}
}
