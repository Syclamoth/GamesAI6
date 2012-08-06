using UnityEngine;
using System.Collections;

public interface IHearable : ILocatable, ISensable {
	//Andrew Dunn 30 July 2012
	//Returns the volume of the object at the given time.
	//makes more sense using the observer pattern.
	Volume getVolume();
}
