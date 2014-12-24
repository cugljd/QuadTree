using System;
using System.Collections.Generic;
using C3.XNA;
using Microsoft.Xna.Framework;

namespace C3.XNA
{
	internal class PerformanceTester
	{
		private static int quadTreeWidth = 10000;
		private static int quadTreeHeight = 10000;
		private static int objectWidth = 10;
		private static int objectHeight = 10;


		private static void Main()
		{
			Console.WriteLine("1)\tAdd stationary objects then delete them");
			Console.WriteLine("2)\tAdd moving objects using Remove/Insert with no movement knowledge");
			Console.WriteLine("3)\tAdd moving objects using Remove/Insert with movement knowledge");
			Console.WriteLine("4)\tAdd moving objects using Moved() with no movement knowledge");
			Console.WriteLine("5)\tAdd moving objects using Moved() with movement knowledge");
			Console.WriteLine("Q)\tQuit");

			Console.WriteLine();
			Console.WriteLine();
			Console.Write("Select an option: ");

			int selection = -1;
			while (selection <= 0)
			{
				int key = Console.ReadKey(true).KeyChar;
				if (key == 'q' || key == 'Q')
				{
					Console.Write('q');
					return;
				}
				if (key >= '1' && key <= '5')
				{
					selection = key;
				}
			}
			Console.Write((char)selection);


			int objectCount = -1;
			while (objectCount <= 0)
			{
				Console.WriteLine();
				Console.WriteLine();
				Console.Write("How many objects do you want to test with? ");
				String line = Console.ReadLine();
				if (line.ToLower().StartsWith("q"))
				{
					return;
				}
				int.TryParse(line, out objectCount);
			}



			double movingPercent = 0.5;


			switch (selection)
			{
			case '1':
				TestStraightAddThenDelete(objectCount);
				break;

			case '2':
				TestMovingObjects(objectCount, movingPercent, false, true);
				break;

			case '3':
				TestMovingObjects(objectCount, movingPercent, true, true);
				break;

			case '4':
				TestMovingObjects(objectCount, movingPercent, false, false);
				break;

			case '5':
				TestMovingObjects(objectCount, movingPercent, true, false);
				break;
			}


			Console.WriteLine();
			Console.WriteLine("Press any key to quit");
			Console.ReadKey();
		}


		/// <summary>
		/// Add items, then delete them
		/// </summary>
		/// <param name="objectCount">The number of items to add then delete</param>
		private static void TestStraightAddThenDelete(int objectCount)
		{
			Random rand = new Random();
			List<SimpleObject> objects = new List<SimpleObject>();
			QuadTree<SimpleObject> quadtree = new QuadTree<SimpleObject>(0, 0, quadTreeWidth, quadTreeHeight);

			// First, create a bunch of objects off the clock, we're trying to test the QuadTree performance here
			for (int i = 0; i < objectCount; i++)
			{
				int x = rand.Next(0, quadTreeWidth - objectWidth);
				int y = rand.Next(0, quadTreeHeight - objectHeight);
				SimpleObject newObj = new SimpleObject(x, y, objectWidth, objectHeight);
				objects.Add(newObj);
			}

			// Start the timer
			DateTime startTime = DateTime.Now;

			// Now add all the objects to the quad tree
			foreach (SimpleObject obj in objects)
			{
				quadtree.Add(obj);
			}
			Console.WriteLine("Adding {0} objects took {1}ms", objectCount, DateTime.Now.Subtract(startTime).TotalMilliseconds);


			// Randomize the list of objects for deletion, not timed
			SimpleObject temp = null;
			for (int i = 0; i < objectCount; i++)
			{
				int otherObjectIndex = rand.Next(0, objectCount);
				if (otherObjectIndex != i)
				{
					// Swap with this other object
					temp = objects[i];
					objects[i] = objects[otherObjectIndex];
					objects[otherObjectIndex] = temp;
				}
			}


			startTime = DateTime.Now;
			// Now lets go and delete them all
			for (int i = objectCount - 1; i >= 0; i--)
			{
				SimpleObject obj = objects[i];
				quadtree.Remove(obj);
				//quadtree.DeleteOLD(obj);
			}
			Console.WriteLine("Removing {0} objects took {1}ms", objectCount, DateTime.Now.Subtract(startTime).TotalMilliseconds);

			if (quadtree.Count != 0)
			{
				throw new Exception("The QuadTree wasn't completely destroyed!");
			}
		}


		/// <summary>
		/// Add a bunch of objects, then make a percentage of them move a random amount
		/// </summary>
		/// <param name="objectCount">The number of objects to add</param>
		/// <param name="movingPercentage">The percentage of those objects to move, expressed as a number between 0.0 and 1.0</param>
		/// <param name="useHasMoveKnowledge">Should we know when an objects moves, or just assume they always move?</param>
		/// <param name="useDeleteInsert">Use the delete/insert method of moving</param>
		private static void TestMovingObjects(int objectCount, double movingPercentage, bool useHasMoveKnowledge, bool useDeleteInsert)
		{
			Random rand = new Random();
			List<SimpleObject> objects = new List<SimpleObject>();
			QuadTree<SimpleObject> quadtree = new QuadTree<SimpleObject>(0, 0, quadTreeWidth, quadTreeHeight);

			// First, create a bunch of objects off the clock, we're trying to test the QuadTree performance here
			for (int i = 0; i < objectCount; i++)
			{
				int x = rand.Next(0, quadTreeWidth - objectWidth);
				int y = rand.Next(0, quadTreeHeight - objectHeight);
				SimpleObject newObj = new SimpleObject(x, y, objectWidth, objectHeight);
				objects.Add(newObj);
			}

			// Start the timer
			DateTime startTime = DateTime.Now;

			// Now add all the objects to the quad tree
			foreach (SimpleObject obj in objects)
			{
				quadtree.Add(obj);
			}
			Console.WriteLine("Adding {0} objects took {1}ms", objectCount, DateTime.Now.Subtract(startTime).TotalMilliseconds);


			// Set up velocities for the objects, not timed
			int maxMovementAmount = 8;
			foreach (SimpleObject obj in objects)
			{
				if (movingPercentage >= 1.0 || rand.NextDouble() > movingPercentage)
				{
					// This object will move
					obj.velocity = new Vector2((float)(rand.NextDouble() * maxMovementAmount * 2) - maxMovementAmount,
					                           (float)(rand.NextDouble() * maxMovementAmount * 2) - maxMovementAmount);
				}
			}


			// Now lets go and move everything using the remove/add technique
			int moveCount = 100;
			startTime = DateTime.Now;
			for (int i = 0; i < moveCount; i++)
			{
				for (int iObj = 0; iObj < objects.Count; iObj++)
				{
					SimpleObject obj = objects[iObj];
					obj.Update();

					if (!useHasMoveKnowledge || obj.HasMoved)
					{
						if (useDeleteInsert)
						{
							quadtree.Remove(obj);
							quadtree.Add(obj);
						}
						else
						{
							//*
							quadtree.Move(obj);
							/*/
							quadtree.Move(iObj);
							//*/
						}
					}
				}
			}

			Console.WriteLine("Moving {0} objects {1} times took {2}ms", objectCount, moveCount, DateTime.Now.Subtract(startTime).TotalMilliseconds);
		}
	}
}