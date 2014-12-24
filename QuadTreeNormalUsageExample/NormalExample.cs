using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace QuadTreeNormalUsageExample
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class NormalExample : Microsoft.Xna.Framework.Game
	{
		private const int OBJECT_COUNT = 100;
		private const int MOVING_OBJECT_COUNT = 5;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private QuadTree<SimpleObject> quadTree;

		private Vector2 viewportSize = new Vector2(200, 200);


		public NormalExample()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 768;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// The values used here would indicate the maximum area of your playing field, in this case, I'm using the window bounds
			quadTree = new QuadTree<SimpleObject>(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

			Random rand = new Random();

			// Make a bunch of objects
			for (int i = 0; i < OBJECT_COUNT + MOVING_OBJECT_COUNT; i++)
			{
				// Randomize the location and size of the simple objects
				int width = rand.Next(2, 12) + rand.Next(2, 12);
				int height = width;		// rectangles will work too, I just like squares
				int x = rand.Next(0, quadTree.QuadRect.Width - width);
				int y = rand.Next(0, quadTree.QuadRect.Height - height);

				// Add our object to the QuadTree and nothing else (the quad tree IS your "list" of objects)
				SimpleObject newObject = new SimpleObject(x, y, width, height);
				quadTree.Add(newObject);

				// Make the moving objects
				if(i >= OBJECT_COUNT)
				{
					newObject.velocity = new Vector2(rand.Next(-5, 5), rand.Next(-5, 5));
				}
			}


			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState()[Keys.Escape].Equals(KeyState.Down))
				this.Exit();

			// Allows dynamic resizing of the viewport
			if (Keyboard.GetState().IsKeyDown(Keys.Q))
			{
				viewportSize.X += 1;
				viewportSize.Y += 1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				if (viewportSize.X >= 2)
				{
					viewportSize.X -= 1;
					viewportSize.Y -= 1;
				}
			}

			
			// Update all of our game objects
			foreach(SimpleObject obj in quadTree)
			{
				obj.Update();

				// If they have moved, we need to update its position in the QuadTree
				if(obj.HasMoved)
				{
					// This logic would normally be done elsewhere, but I'm cheap, so I'm making any moving objects wrap here
					// Wrap along the X-axis
					if(obj.Position.X < 0)
					{
						obj.Position = new Vector2(quadTree.QuadRect.Width - obj.Rect.Width, obj.Position.Y);
					}
					else if(obj.Position.X + obj.Rect.Width > quadTree.QuadRect.Width)
					{
						obj.Position = new Vector2(0, obj.Position.Y);
					}

					// Wrap along the Y-axis
					if (obj.Position.Y < 0)
					{
						obj.Position = new Vector2(obj.Position.X, quadTree.QuadRect.Height - obj.Rect.Height);
					}
					else if (obj.Position.Y + obj.Rect.Height > quadTree.QuadRect.Height)
					{
						obj.Position = new Vector2(obj.Position.X, 0);
					}

					// This is the important part, without this, the objects would remain in the quad they started in, try it!
					quadTree.Move(obj);
				}
			}


			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			MouseState mouse = Mouse.GetState();

			// This would normally be defined by your window size, but for this example, it is attached to the mouse
			Rectangle viewport = new Rectangle((int)(mouse.X - (viewportSize.X / 2.0)),
			                                   (int)(mouse.Y - (viewportSize.Y / 2.0)),
			                                   (int)viewportSize.X,
			                                   (int)viewportSize.Y);

			// This will retrieve a list of objects that are within ut viewport
			List<SimpleObject> visibleObjects = quadTree.GetObjects(viewport);
			foreach (SimpleObject obj in visibleObjects)
			{
				// Normally you'd ask the Object to draw itself, but to keep this example simple, we're going to draw for it using the Primitives2D library
				Primitives2D.FillRectangle(spriteBatch, obj.Rect, Color.DarkRed);
			}

			// Now draw the viewport
			Primitives2D.DrawRectangle(spriteBatch, viewport, Color.Black);

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
