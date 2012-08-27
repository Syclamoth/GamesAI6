using UnityEngine;
using System.Collections;

public static class Extensions {
	public static bool IntersectLine(this Rect rectangle, Vector2 lineStart, Vector2 lineEnd, out Vector2 intersectPoint) {
		float minX = lineStart.x < lineEnd.x ? lineStart.x : lineEnd.x;
		float maxX = lineStart.x > lineEnd.x ? lineStart.x : lineEnd.x;
		
		intersectPoint = Vector2.zero;
		
		if(maxX > rectangle.xMax)
		{
			maxX = rectangle.xMax;
		}
		if(minX < rectangle.xMin)
		{
			minX = rectangle.xMin;
		}
		
		if(minX > maxX) 
		{
			return false;
		}
		
		float minY = lineStart.y;
		float maxY = lineEnd.y;
		
		float dx = lineEnd.x - lineStart.x;
		
		if(Mathf.Abs(dx) > 0.0001)
		{
			float a = (lineEnd.y - lineStart.y) / dx;
			float b = lineStart.y - (a * lineStart.x);
			minY = a * minX + b;
			maxY = a * maxX + b;
		}
		
		if(minY > maxY) {
			float tmp = maxY;
			maxY = minY;
			minY = tmp;
		}
		
		if(maxY > rectangle.yMax) {
			maxY = rectangle.yMax;
		}
		
		if(minY < rectangle.yMin) {
			minY = rectangle.yMin;
		}
		if(minY > maxY) {
			return false;
		}
		
		float desiredDistance = 20;
		
		intersectPoint = rectangle.center - ((rectangle.center - lineStart).normalized * desiredDistance);
		
		intersectPoint.x = lineStart.x < lineEnd.x ? minX : maxX;
		intersectPoint.y = lineStart.y < lineEnd.y ? minY : maxY;
		//Debug.Log ("Min: " + new Vector2(minX, minY) + " Max: " + new Vector2(maxX, maxY));
		
		return true;
	}
	
	//public static Vector2 LineIntersection(
	public static Rect OffsetBy(this Rect rectangle, Vector2 offset) {
		Rect retV = new Rect(rectangle);
		retV.x += offset.x;
		retV.y += offset.y;
		
		return retV;
	}
	
	public static Vector3[] GetCorners(this Rect rectangle) {
		return new Vector3[] {
			new Vector3(rectangle.xMin, rectangle.yMin, 0),
			new Vector3(rectangle.xMax, rectangle.yMin, 0),
			new Vector3(rectangle.xMax, rectangle.yMax, 0),
			new Vector3(rectangle.xMin, rectangle.yMax, 0)
		};
	}
	
	public static Vector3 ToWorldCoords(this Vector2 vector) {
		return new Vector3(vector.x, 0, vector.y);
	}
	
	public static bool IntersectCircle(this Rect rectangle, Vector2 centre, float radius) {
		// Naive pass if the rectangle contains the centre of the circle:
		if(rectangle.Contains(centre)) {
			return true;
		}
		// Naive fail if the centre of the circle is further away from the edge than radius:
		if(centre.x > rectangle.xMax + radius || centre.x < rectangle.xMin - radius || centre.y > rectangle.yMax + radius || centre.y < rectangle.yMin - radius) {
			return false;
		}
		
		if(centre.x < rectangle.xMax && centre.x > rectangle.xMin) {
			// is within x axis, so passes
			return true;
		}
		if(centre.y < rectangle.yMax && centre.y > rectangle.yMin) {
			// is within y axis, so passes
			return true;
		}
		// Square the radius ahead of time to avoid a sqrt operation
		float sqrRadius = radius * radius;
		// Check if the circle intersects each corner
		foreach(Vector2 corner in rectangle.GetCorners()) {
			if((corner - centre).sqrMagnitude < sqrRadius) {
				return true;
			}
		}
		
		return false;
	}

}

public static class ColourUtility {
	
	public static Color GetSpectrum(float point)
	{
		float r = Mathf.Clamp01(Mathf.Sin(point));
		float g = Mathf.Clamp01(Mathf.Cos(point + (0.66f * Mathf.PI)));
		float b = Mathf.Clamp01(Mathf.Sin(point + (1.33f * Mathf.PI)));
		return new Color(r, g, b, 1f);
	}
}