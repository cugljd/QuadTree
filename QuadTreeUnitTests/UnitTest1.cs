using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using C3.XNA;

namespace QuadTreeUnitTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		public UnitTest1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestMethod1()
		{
			QuadTree<SimpleObject> quadTree = new QuadTree<SimpleObject>(0, 0, 1000, 1000);
			const int OBJ_SIZE = 100;
			
			List<SimpleObject> objs = new List<SimpleObject>();
			objs.Add(new SimpleObject(50, 50, OBJ_SIZE, OBJ_SIZE));				// 0
			objs.Add(new SimpleObject(450, 450, OBJ_SIZE, OBJ_SIZE));			// 1
			objs.Add(new SimpleObject(501, 100, OBJ_SIZE, OBJ_SIZE));			// 2
			
			foreach(SimpleObject obj in objs)
			{
				quadTree.Add(obj);
			}

			List<SimpleObject> results = new List<SimpleObject>();
			quadTree.GetObjects(new Rectangle(0, 0, 51, 51), ref results);
			Assert.AreEqual(1, results.Count);
			Assert.AreSame(objs[0], results[0]);
			Assert.AreEqual(0, quadTree.GetObjects(new Rectangle(0, 0, 49, 50)).Count);
			Assert.AreEqual(0, quadTree.GetObjects(new Rectangle(0, 0, 50, 49)).Count);
			
			// Make sure the program doesn't crash when we access a rectangle outside the bounds
			Assert.AreEqual(0, quadTree.GetObjects(new Rectangle(-1, -1, 2, 2)).Count);
			Assert.AreSame(objs[0], quadTree.GetObjects(new Rectangle(-100, -100, 300, 300))[0]);
			
			// Make sure that we can detect an object from a rectangle that is contained by the child
			Assert.AreSame(objs[0], quadTree.GetObjects(new Rectangle(55, 55, 1, 1))[0]);
			
		}
	}
}
