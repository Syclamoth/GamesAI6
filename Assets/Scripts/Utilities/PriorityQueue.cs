using System.Collections;
using System.Collections.Generic;
//Unit tested August 4, 2012.
//By Andrew Dunn
//A very efficient priority queue (FIFO with priority) implementing a heap
//data structure featuring O(log n) queues and dequeues.
//Nodes with a higher priority will move to the front.
//Implements ICollection<T> to better be used by code
//Would be useful for things like the sensory system, where louder noises
//are processed first
public class PriorityQueue<T> : ICollection<T> {
	//Nodes stored in resizable random accessable array.
	private List<PriorityEnqueuedItem<T>> nodes;
	
	//Need to separately track number of nodes in queue, as nodes.RemoveAt()
	//are not used for efficiency reasons.
	private int count;
	
	//The priority used when the user uses the ICollection.Add() method
	public double DefaultPriority = 0;
	
	//O(1)
	//Initializes the list
	public PriorityQueue() {
		this.nodes = new List<PriorityEnqueuedItem<T>>();
		count = 0;
	}
	
	//O(log n)
	//Adds a PriorityEnqueuedItem<T> to the Queue, item's with a higher
	//priority are automatically sent to the front of the queue.
	public void enqueue(PriorityEnqueuedItem<T> item) {
		int index = count++;
		int parent;
		PriorityEnqueuedItem<T> temp;
		nodes.Add (item);
		
		/* Works by adding the new node to the end of the tree, then swapping 
		 * it with lower valued higher branches until in the correct position.
		 * This ensures that every nodes children's value is lower than itself.
		 */
		for (;;) {
			if (index == 0)
				return;
			parent = (index-1)/2;
			
			//If the parent's priority is lower, swap the values.
			if (nodes[parent].priority < nodes[index].priority) {
				temp = nodes[index];
				nodes[index] = nodes[parent];
				nodes[parent] = temp;
			}
			else
			{
				//Otherwise the heap is correctly structured
				return;
			}
			//Next index.
			index = parent;
		}
	}
	
	public void enqueueWithPriority(T item,double priority)
	{
		this.enqueue (new PriorityEnqueuedItem<T>(item,priority));
	}
	
	/*public T dequeue() {
		int child1,index = 0;
		T returnMe;
		PriorityEnqueuedItem<T> temp;
		//Edge case for empty queue
		if (count == 0) {
			return default(T); //Return null
		}
		
		//Edge case for queue with 1 node
		if (count == 1) {
			//Empty queue
			count = 0;
			return nodes[0].item;
		}
		returnMe = nodes[0].item;
		
		nodes[0] = nodes[--count];
		for (;;) {
			child1 = index*2+1;
			//Make sure the child index is not out of bounds. if it is then the
			//heap is correctly organized
			if (child1 >= count) {
				return returnMe;
			}
			//Only need to worry about first child as second is out of bounds.
			if (child1 + 1 == count) {
				//Children are smaller than moved node, heap is correctly
				//sorted
				if (nodes[index].priority > nodes[child1].priority) {
					return returnMe;
				}
				//Swap node with left child node
				temp = nodes[child1];
				nodes[child1] = nodes[index];
				nodes[index] = temp;
				index = child1;
				continue;
			}
			//Current node has 2 children, will swap it with the larger of the 2.
			if (nodes[child1].priority <= nodes[child1+1].priority) {
				child1 = child1+1;
			}
			
			//Children are smaller than moved node, heap is correctly
			//sorted
			if (nodes[index].priority > nodes[child1].priority) {
				return returnMe;
			}
			
			//Swap node with greatest child node
			temp = nodes[child1];
			nodes[child1] = nodes[index];
			nodes[index] = temp;
			index = child1;
		}
	}*/
	
	//O(log n)
	public T dequeue() {
		return this.removeAt (0);
	}
	
	//O(log n)
	public T removeAt(int index)
	{
		int child1;
		T returnMe;
		PriorityEnqueuedItem<T> temp;
		//Edge case for empty queue or out of bounds
		if (count == 0) {
			throw new System.InvalidOperationException("Queue is empty.");
		}
		
		if (index >= count) {
			throw new System.InvalidOperationException("Out of bounds.");
		}
		
		//Edge case for queue with 1 node
		if (count == 1) {
			//Empty queue
			count = 0;
			returnMe = nodes[0].item;
			nodes.RemoveAt(count);
			return returnMe;
		}
		returnMe = nodes[index].item;
		
		nodes[index] = nodes[--count];
		nodes.RemoveAt(count);
		for (;;) {
			child1 = index*2+1;
			//Make sure the child index is not out of bounds. if it is then the
			//heap is correctly organized
			if (child1 >= count) {
				return returnMe;
			}
			//Only need to worry about first child as second is out of bounds.
			if (child1 + 1 == count) {
				//Children are smaller than moved node, heap is correctly
				//sorted
				if (nodes[index].priority > nodes[child1].priority) {
					return returnMe;
				}
				//Swap node with left child node
				temp = nodes[child1];
				nodes[child1] = nodes[index];
				nodes[index] = temp;
				index = child1;
				continue;
			}
			//Current node has 2 children, will swap it with the larger of the 2.
			if (nodes[child1].priority <= nodes[child1+1].priority) {
				child1 = child1+1;
			}
			
			//Children are smaller than moved node, heap is correctly
			//sorted
			if (nodes[index].priority > nodes[child1].priority) {
				return returnMe;
			}
			
			//Swap node with greatest child node
			temp = nodes[child1];
			nodes[child1] = nodes[index];
			nodes[index] = temp;
			index = child1;
		}
	}
	
	//ICollection Members
	public int Count
	{
		get
		{
			return this.count;
		}
	}
	
	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}
	
	//O(log n)
	public void Add (T item)
	{
		this.enqueue (new PriorityEnqueuedItem<T>(item,DefaultPriority));
	}
	
	//O(1)
	public void Clear ()
	{
		this.count = 0;
	}
	
	//O(n)
	public bool Contains (T item)
	{
		int i;
		//Loop through until an item is matched. Otherwise terminate if no
		//match found
		for (i=0;i<count;i++) {
			if (nodes[i].item.Equals (item))
				return true;
		}
		return false;
	}
	
	//O(n)
	public void CopyTo (T[] array,int arrayIndex)
	{
		int i;
		//Loop through and copy all to array
		for (i=0;i<count;i++) {
			array[arrayIndex+i] = nodes[i].item;
		}
	}
	
	//O(n)
	public bool Remove (T item)
	{
		int i;
		for (i=0;i<count;i++)
		{
			if (nodes[i].item.Equals (item)) {
				if (this.removeAt(i).Equals(default(T))) {
					return false;
				}
				return true;
			}
		}
		return false;
	}
	
	public IEnumerator<T> GetEnumerator() {
		return new PriorityQueueEnumerator<T>(this);
	}
	
	IEnumerator IEnumerable.GetEnumerator() {
		return new PriorityQueueEnumerator<T>(this);
	}
}

/*Immutable to ensure priority never changes
 *Stores reference to enqueued item as well as an integer priority
 */
public struct PriorityEnqueuedItem<T> {
	public T item {get; private set;}
	public double priority {get; private set;}
	
	public PriorityEnqueuedItem(T item,double priority) : this()
	{
		this.item = item;
		this.priority = priority;
	}
}

/* Makes a copy of the queue to be enumerated and then allows it to be
 * enumerated in any particular order*/
public class PriorityQueueEnumerator<T> : IEnumerator<T>
{
	T[] queue;
	int i;
	public PriorityQueueEnumerator(PriorityQueue<T> priorityQueue)
	{
		this.queue = new T[priorityQueue.Count];
		priorityQueue.CopyTo (this.queue,0);
		this.Reset ();
	}
	
	public T Current
	{
		get {
			return queue[i];
		}
	}
	
	object IEnumerator.Current
	{
		get {
			return queue[i];
		}
	}
	
	//
	public bool MoveNext()
	{
		return ++i < queue.Length;
	}
	
	//Set the position of i.
	public void Reset()
	{
		i = -1;
	}
	
	//For some reason I need to implement this
	public void Dispose()
	{
		//Set queue to null.
		queue = default(T[]);
	}
}