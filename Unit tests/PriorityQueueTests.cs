using System;
using NUnit.Framework;
namespace AssemblyCSharp
{
	[TestFixture()]
	public class PriorityQueueTests
	{
		[Test()]
		public void TestEnqueueDequeue ()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();
			queue.enqueueWithPriority (16,4);
			queue.enqueueWithPriority (14,5);
			queue.enqueueWithPriority (25,3);
			queue.enqueueWithPriority (17,4);
			
			Assert.AreEqual (queue.dequeue (), 14);
			Assert.AreEqual (queue.dequeue (), 16);
			Assert.AreEqual (queue.dequeue (), 17);
			Assert.AreEqual (queue.dequeue (), 25);
			
			queue.enqueueWithPriority(11,1);
			
			Assert.AreEqual (queue.dequeue (), 11);
			
			try {
				Console.Out.WriteLine("Attempting to dequeue an empty queue.");
				queue.dequeue();
				Console.Error.WriteLine("FAIL: System.InvalidOperationExcepti"+
					         "on was not thrown when dequeing an empty queue");
				Assert.IsTrue(false);
			} catch (System.InvalidOperationException e)
			{
				Console.Out.WriteLine("PASS: System.InvalidOperationException"+
					                        " thrown as expected: "+e.Message);
				Assert.IsTrue(true,"Exception thrown");
			}
		}
	}
}

