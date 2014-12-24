using C3.XNA;
using Microsoft.Xna.Framework;

namespace QuadTreeUnitTests
{
	class SimpleObject : IQuadStorable
	{
		private Vector2 position;
		private int w, h;
		private Vector2 velocity;
		
		public SimpleObject(int theX, int theY, int theW, int theH)
		{
			position.X = theX;
			position.Y = theY;
			w = theW;
			h = theH;
		}
		
		
		public void Update()
		{
			// TODO: Move this object
		}
		
		
		public Rectangle Rect
		{
			get
			{
				return new Rectangle((int)(position.X + 0.5), (int)(position.Y + 0.5), w, h);
			}
		}

		public bool HasMoved
		{
			get { return false; }
		}
	}
}
