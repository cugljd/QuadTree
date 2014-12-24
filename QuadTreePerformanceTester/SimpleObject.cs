using System;
using C3.XNA;
using Microsoft.Xna.Framework;

namespace C3.XNA
{
	public class SimpleObject : IQuadStorable
	{
		private Vector2 position;
		private int w, h;
		public Vector2 velocity;

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}
		
		public SimpleObject(int theX, int theY, int theW, int theH)
		{
			position.X = theX;
			position.Y = theY;
			w = theW;
			h = theH;
		}


		public void Update()
		{
			if (velocity.X != 0.0 || velocity.Y != 0.0)
			{
				position.X += velocity.X;
				position.Y += velocity.Y;
				HasMoved = true;
			}
			else
			{
				HasMoved = false;
			}
		}


		/// <summary>
		/// The rectangle that defines the object's boundaries.
		/// </summary>
		public Rectangle Rect
		{
			get
			{
				return new Rectangle((int)(position.X + 0.5), (int)(position.Y + 0.5), w, h);
			}
		}

		/// <summary>
		/// This should return True if the object has moved during the last update, false otherwise
		/// </summary>
		public bool HasMoved
		{
			get;
			private set;
		}

		#region IComparable<DrawableObject> Members
		//int IComparable<SimpleObject>.CompareTo(SimpleObject other)
		//{
		//    return (int)(position.Y + h) - (int)(other.position.Y + other.h);
		//}
		#endregion

		#region IComparable<IQuadStorable> Members

		//public int CompareTo(IQuadStorable other)
		//{
		//    return (int)(position.Y + h) - (int)(other.Rect.Y + other.Rect.Height);
		//}

		#endregion
	}
}