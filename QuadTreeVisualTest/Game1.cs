/* NOTES:
 * ------
 * This is a quick and dirty little tester program to make sure the QuadTree
 * class is functioning as expected, and also to be a cool little visual
 * representation. This is definitely *NOT* intended to be a hallmark of coding
 * excellence, as demonstrated by the mess of commented code. However, it does
 * provide various functionality to test a quad tree.
 * 
 * The visual representation should be apparent upon running (though please
 * note that there is a toggle you can set for "stress mode" or "normal" mode),
 * however; there is also some functionality to describe...
 * 
 *   + Moving the mouse moves the "view port" window around. Any objects that
 *     fall within this view port will have their colour changed. Please note
 *     that when the object count exceeds 1000, objects outside the viewport
 *     are not drawn at all.
 *     
 *   + The view port can be expanded and contracted with the Q and A keys.
 *   
 *   + Objects can be added by left-clicking anywhere on the window. They can
 *     be deleted by right-clicking on a highlighted object (anything that
 *     falls under the "cross hair", ie, the circle with the dot in it).
 *   
 *   + If the object count is less than 1000, a visual representation of the
 *     quad will be drawn (using colours, whoo!) which will update as objects
 *     are created and destroyed.
 */
using System;
using System.Collections.Generic;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace QuadTreeVisualTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const int SCR_WIDTH = 1024;
        private const int SCR_HEIGHT = 768;

        private const int QUAD_WIDTH = 1024;
        private const int QUAD_HEIGHT = 768;

        // NOTE: The "super comment" below toggles the mode
        //       //* toggles stress mode
        //       /*  toggles normal mode
        // Thanks to John McDonald for this aptly named trick :)

        /*
        
        // Stress Mode -- Tests performance with a high object count to see
        //                the effects of a high object count.
        
        private const int OBJECT_COUNT = 100000;       
        private Vector2 m_objSize = new Vector2(2, 2);

        /*/
        
        // Normal Mode -- Populates the quad tree with a few objects so we can
        //                see the quad draw and watch it react as objects are
        //                created and destroyed
        
        private const int OBJECT_COUNT = 25;
        private Vector2 m_objSize = new Vector2(25, 25);

        //*/

		private QuadTree<SimpleObject> m_quadTree = new QuadTree<SimpleObject>(new Rectangle(0, 0, QUAD_WIDTH, QUAD_HEIGHT));
		private List<SimpleObject> m_objList = new List<SimpleObject>();
		private List<SimpleObject> m_drawList = new List<SimpleObject>();
		private List<SimpleObject> m_highlightedList = new List<SimpleObject>();

        private SpriteFont m_spriteFont = null;

        private Vector2 m_cursorPos = new Vector2(0, 0);
        private Rectangle m_region = new Rectangle(0, 0, 200, 200);
        private Rectangle m_mouseHighlightRect = new Rectangle(0, 0, 1, 1);

        private MouseState m_oldMS;

        private Texture2D m_spriteTexture;

		private int m_frameRate = 0;
		private int m_frameCounter = 0;
		private TimeSpan m_elapsedTime = TimeSpan.Zero;

		private SimpleObject m_movingObject = new SimpleObject(80, 50, 30, 30);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            m_oldMS = Mouse.GetState();

            //m_mouseHighlightRect.Width = (int)m_objSize.X;
            //m_mouseHighlightRect.Height = (int)m_objSize.Y;

            graphics.PreferredBackBufferWidth = SCR_WIDTH;
            graphics.PreferredBackBufferHeight = SCR_HEIGHT;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            //Primative.Initialize(graphics.GraphicsDevice, spriteBatch);

            //*
			SimpleObject obj1 = new SimpleObject(5, 5, (int)m_objSize.X, (int)m_objSize.Y);
			SimpleObject obj2 = new SimpleObject(5, 125, (int)m_objSize.X, (int)m_objSize.Y);
			SimpleObject obj3 = new SimpleObject(300, 125, (int)m_objSize.X, (int)m_objSize.Y);
			SimpleObject obj4 = new SimpleObject(400, 125, (int)m_objSize.X, (int)m_objSize.Y);

            m_objList.Add(obj1);
            m_objList.Add(obj2);
			m_objList.Add(obj3);
            m_objList.Add(obj4);

			//m_objList.Add(m_movingObject);

			foreach (SimpleObject o in m_objList)
			{
				m_quadTree.Add(o);
			}

			m_quadTree.Add(m_movingObject);
			m_objList.Add(m_movingObject);

            /*/

            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < OBJECT_COUNT; i++)
            {
				SimpleObject obj = new SimpleObject(
										r.Next(0, m_quadTree.QuadRect.Width - (int)m_objSize.X),
										r.Next(0, m_quadTree.QuadRect.Height - (int)m_objSize.Y),
										(int)m_objSize.X,
										(int)m_objSize.Y);
                m_objList.Add(obj);
				m_quadTree.Insert(obj);
            }
            //*/

            Console.WriteLine("--- Init Finished ---");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            m_spriteFont = Content.Load<SpriteFont>("SpriteFont1");

            m_spriteTexture = Content.Load<Texture2D>("Solid");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
          
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            m_cursorPos.X = ms.X;
            m_cursorPos.Y = ms.Y;

            m_region.X = ms.X - m_region.Width / 2;
            m_region.Y = ms.Y - m_region.Height / 2;

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                m_region.Width += 1;
                m_region.Height += 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (m_region.Width >= 2)
                {
                    m_region.Width -= 1;
                    m_region.Height -= 1;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                m_quadTree.Clear();
                m_objList.Clear();
                m_highlightedList.Clear();
            }

			if (Keyboard.GetState().IsKeyDown(Keys.Z))
			{
				m_movingObject.Position = new Vector2(m_movingObject.Position.X - 5, m_movingObject.Position.Y);
				
				m_quadTree.Move(m_movingObject);
				//m_quadTree.Delete(m_movingObject);
				//m_quadTree.Insert(m_movingObject);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.X))
			{
				m_movingObject.Position = new Vector2(m_movingObject.Position.X + 5, m_movingObject.Position.Y);
				m_quadTree.Move(m_movingObject);
				//m_quadTree.Delete(m_movingObject);
				//m_quadTree.Insert(m_movingObject);
			}

            if (ms.LeftButton == ButtonState.Pressed && m_oldMS.LeftButton != ButtonState.Pressed)
            {
                if (ms.X >= 0 && ms.Y >= 0 && ms.X <= SCR_WIDTH && ms.Y <= SCR_HEIGHT)
                {
					SimpleObject newObj = new SimpleObject(
                        (int)(m_cursorPos.X - m_objSize.X / 2.0),
						(int)(m_cursorPos.Y - m_objSize.Y / 2.0),
						(int)m_objSize.X,
						(int)m_objSize.Y
                        );

                    m_objList.Add(newObj);
                    m_quadTree.Add(newObj);
                }
            }

            if (ms.RightButton == ButtonState.Pressed && m_oldMS.RightButton != ButtonState.Pressed)
            {
				foreach (SimpleObject o in m_highlightedList)
                {
                    m_objList.Remove(o);
                    m_quadTree.Remove(o);
                }
            }

            if (ms != m_oldMS)
            {
                m_mouseHighlightRect.X = (int)(m_cursorPos.X - m_mouseHighlightRect.Width / 2);
                m_mouseHighlightRect.Y = (int)(m_cursorPos.Y - m_mouseHighlightRect.Height / 2);
                m_highlightedList.Clear();
                m_quadTree.GetObjects(m_mouseHighlightRect, ref m_highlightedList);
            }
            
            m_oldMS = ms;

			
			// Do more accurate frame rate counts
			m_elapsedTime += gameTime.ElapsedGameTime;

			if (m_elapsedTime > TimeSpan.FromSeconds(0.25))
			{
				m_elapsedTime -= TimeSpan.FromSeconds(0.25);
				m_frameRate = m_frameCounter * 4;
				m_frameCounter = 0;
			}           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			m_frameCounter++;
			
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

			//QuadTree<SimpleObject>.QuadTreeObject obj = new SimpleObject(125, 125, 40, 40);

            if (m_objList.Count <= 1000)
            {
				foreach (SimpleObject o in m_objList)
                {
                    //Primative.FilledBox(o.WorldPosition, o.WorldPosition + o.Size, Color.Pink);
                    //Primative.Box(o.WorldPosition, o.WorldPosition + o.Size, 2, Color.Red);

                    //*
					spriteBatch.Draw(m_spriteTexture, o.Rect, new Rectangle(0, 0, o.Rect.Width, o.Rect.Height), Color.Pink);
                    /*/
                    o.Draw(gameTime, Color.Pink);
                    //*/
                }
#if DEBUG
                DrawQuad(m_quadTree, 0);
#endif
            }

            m_drawList.Clear();
            /*
            m_quadTree.GetYOrdered(m_region, ref m_drawList);
            /*/
            m_quadTree.GetObjects(m_region, ref m_drawList);
            //*/
            
            //m_drawList.Sort();


			foreach (SimpleObject o in m_drawList)
            {
                //Primative.FilledBox(o.WorldPosition, o.WorldPosition + o.Size, Color.RosyBrown);
                //Primative.Box(o.WorldPosition, o.WorldPosition + o.Size, 2, Color.SlateGray);

                //*
                spriteBatch.Draw(m_spriteTexture, o.Rect, new Rectangle(0, 0, o.Rect.Width, o.Rect.Height), Color.SlateGray);
                /*/
                o.Draw(gameTime, Color.SlateGray);
                //*/
            }

			foreach (SimpleObject o in m_highlightedList)
            {
				spriteBatch.Draw(m_spriteTexture, o.Rect, new Rectangle(0, 0, o.Rect.Width, o.Rect.Height), Color.White);
                //o.Draw(gameTime, Color.White);
            }

			//Primative.Box(m_region.Left, m_region.Top, m_region.Right, m_region.Bottom, 1, Color.Tomato);
			//Primative.Circle(m_region.Left + m_region.Width / 2, m_region.Top + m_region.Height / 2, 5, Color.Tomato);
			//Primative.PutPixel(m_region.Left + m_region.Width / 2, m_region.Top + m_region.Height / 2, Color.Tomato);
        	Primitives2D.DrawRectangle(spriteBatch, m_region, Color.Tomato, 1);
            Primitives2D.DrawCircle(spriteBatch,
        	                        new Vector2(m_region.Left + m_region.Width/2, m_region.Top + m_region.Height/2), 5, 10,
        	                        Color.Tomato);
            Primitives2D.PutPixel(spriteBatch,
        	                       new Vector2(m_region.Left + m_region.Width/2, m_region.Top + m_region.Height/2),
        	                       Color.Tomato);


            string str = String.Format("FPS = {0}, Drawing {1}/{2} Objects (Quad contains {3})",
				m_frameRate, m_drawList.Count, m_objList.Count, m_quadTree.Count);
            spriteBatch.DrawString(m_spriteFont, str, new Vector2(0, 750), Color.White);

			spriteBatch.DrawString(m_spriteFont, "Press Q and A to zoom \"view\" in/out, E to erase all, Left-Click\nto make new, Right-Click to delete, Z/X to move one of\nthe initial cubes left/right, ESC to exit", new Vector2(0, 700), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


		
#if DEBUG
		private void DrawQuad(QuadTree<SimpleObject> quad, int depth)
        {
            if (quad != null)
            {
				DrawQuad(quad.RootQuad, depth);
			}
		}

		private void DrawQuad(QuadTreeNode<SimpleObject> quad, int depth)
		{
			if (quad != null)
			{
                Rectangle rect = quad.QuadRect;

                Point mid = new Point(
                    rect.X + rect.Width / 2,
                    rect.Y + rect.Height / 2
                    );

                Color drawColor = Color.White;
                switch (depth)
                {
                    case 0:
                        drawColor = Color.White;
                        break;
                    case 1:
                        drawColor = Color.Red;
                        break;
                    case 2:
                        drawColor = Color.Green;
                        break;
                    case 3:
                        drawColor = Color.Blue;
                        break;
                    case 4:
                        drawColor = Color.Gray;
                        break;
                    case 5:
                        drawColor = Color.DarkRed;
                        break;
                    case 6: 
                        drawColor = Color.DarkGreen;
                        break;
                    case 7:
                        drawColor = Color.DarkBlue;
                        break;
                    default:
                        drawColor = Color.White;
                        break;
                }

                //Primative.Box(rect.Left, rect.Top, rect.Right, rect.Bottom, 1, drawColor);
                Primitives2D.DrawRectangle(spriteBatch, rect, drawColor, 1);

                DrawQuad(quad.TopLeftChild, depth+1);
                DrawQuad(quad.TopRightChild, depth+1);
                DrawQuad(quad.BottomLeftChild, depth+1);
                DrawQuad(quad.BottomRightChild, depth+1);
            }
        }
#endif
	}

}
