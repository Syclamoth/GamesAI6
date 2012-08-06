using UnityEngine;
using System.Collections.Generic;

/* Manages sounds. All sound emmiters register themselves with the active
 * SoundManager, which can then be observed by any object implementing the
 * IHearing interface. */

public class SoundManager {
	/* Make sure only 1 of these is active at a time. I didn't use the
	 * singleton pattern becuase we'll probably need a different SoundManager
	 * for different levels.
	 */
	
	//Naive for now, will change most likely to a grid of ArrayLists + an index.
	private List<IHearable> hearableObjects;
	
	
	public SoundManager() {
		hearableObjects = new List<IHearable>();
	}
	
	//Makes sure nothing is double added. Anything which can be heard by
	//another must be registered first.
	public void registerHearable(IHearable hearableObject) {
		if (!hearableObjects.Contains (hearableObject)) {
			hearableObjects.Add (hearableObject);
		}
	}
	
	public PriorityQueue<IHearable> getObjectsObservableBy(IHearing listener) {
		//Brute force approach = O(n)
		//For all hearing objects it's O(m*n) ~ Acceptable.
		PriorityQueue<IHearable> queue = new PriorityQueue<IHearable>();
		int i;
		
		IHearable target;
		for (i=0;i<hearableObjects.Count;++i)
		{
			double distanceSquared,priority; //Hopefully this will be optimized out.
			
			target = hearableObjects[i];
			
			//Calculates Distance^2
			distanceSquared = (listener.getLocation() - target.getLocation()).sqrMagnitude;
			
			//Store in variable for use as queue priority
			priority = target.getVolume().volumeFromDistanceSquared(distanceSquared).Intensity;
			
			//Put in queue if louder than hearing threshold.
			if (priority >= listener.getHearingThreshold().Intensity)
			{
				queue.enqueueWithPriority(target,priority);
			}
		}
		
		return queue;
	}
}
