using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Missile_Command
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D explosionTexture;

        Vector2 leftBasePosition;
        Vector2 middleBasePosition;
        Vector2 rightBasePosition;

        float slowMissileSpeed = 2f;
        float fastMissileSpeed = 5f;

        List<Missile> missiles;
        List<Explosion> activeExplosions;
        List<Explosion> shrinkingExplosions;

        MouseState m;
        KeyboardState oldKb;
        //basses implementation
        Rectangle[] missilePos;
        Texture2D missileBase, L;
        Rectangle land1, land2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500 *2;
            graphics.PreferredBackBufferHeight = 500 *2;
            
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

            IsMouseVisible = true;


            missiles = new List<Missile>(20);
            activeExplosions = new List<Explosion>(20);
            shrinkingExplosions = new List<Explosion>(20);

            leftBasePosition = new Vector2(GraphicsDevice.Viewport.Width * 0.15f, GraphicsDevice.Viewport.Height - 50);
            middleBasePosition = new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height - 50);
            rightBasePosition = new Vector2(GraphicsDevice.Viewport.Width * 0.85f, GraphicsDevice.Viewport.Height - 50);
                //control panel
            m = Mouse.GetState();
            oldKb = Keyboard.GetState();
            //bases
            missilePos = new Rectangle[3];
            int framsX = GraphicsDevice.Viewport.Width;
            int framsY = GraphicsDevice.Viewport.Height;
            missilePos[0] = new Rectangle(0, framsY - 100, 125, 100);
            missilePos[1] = new Rectangle((framsX / 2) - 100, framsY - 100, 125, 100);
            missilePos[2] = new Rectangle(framsX - 125, framsY - 100, 125, 100);
            land1 = new Rectangle(missilePos[0].X + missilePos[0].Width, missilePos[0].Y + (int)(missilePos[0].Width / 5),
                        Distance(missilePos[0], missilePos[1]) - missilePos[0].Width, 100);
            land2 = new Rectangle(missilePos[1].X + missilePos[1].Width, missilePos[1].Y + (int)missilePos[1].Width / 5,
                Distance(missilePos[1], missilePos[2]) - missilePos[1].Width, 100);


            

            base.Initialize();
        }
        // method use to find distance between two rectrangles
        //@param 2 Rectangle
        // @return distance cast from double to int
        public int Distance(Rectangle rect1, Rectangle rect2)
        {
            double dis = Math.Sqrt(Math.Pow(rect2.X - rect1.X, 2) + Math.Pow(rect2.Y - rect1.Y, 2));
            return (int)dis;
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
            //exlosion
            explosionTexture = Content.Load<Texture2D>("EFX/efx_explosion_b_0001");
            //bases
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            //missileBase = Content.Load<Texture2D>("missle base");
            //L = Content.Load<Texture2D>("Star");


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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            KeyboardState kb = Keyboard.GetState();

            m = Mouse.GetState();

            ///////////////////////////////////////////////     Firing Logic

            if(kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A))
            {
                missiles.Add(new Missile(Content.Load<Texture2D>("2D/missile_small"), leftBasePosition, slowMissileSpeed, new Vector2(m.X, m.Y)));
            }

            if(kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S))
            {
                missiles.Add(new Missile(Content.Load<Texture2D>("2D/missile_small"), middleBasePosition, fastMissileSpeed, new Vector2(m.X, m.Y)));
            }

            if (kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D))
            {
                missiles.Add(new Missile(Content.Load<Texture2D>("2D/missile_small"), rightBasePosition, slowMissileSpeed, new Vector2(m.X, m.Y)));
            }




            ///////////////////////////////////////////////     Update Logic

            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].Update();
                if(missiles[i].willExplode)
                {
                    activeExplosions.Add(missiles[i].Detonate());
                    missiles.RemoveAt(i);
                    i--;
                }
            }

            for(int i = 0; i < activeExplosions.Count; i++)
            {
                activeExplosions[i].Update();

                if(activeExplosions[i].finishedExpanding)
                {
                    shrinkingExplosions.Add(activeExplosions[i]);
                    activeExplosions.RemoveAt(i);
                    i--;
                }
            }

            for(int i = 0; i < shrinkingExplosions.Count; i++)
            {
                shrinkingExplosions[i].Shrink();

                if(shrinkingExplosions[i].finishedShrinking)
                {
                    shrinkingExplosions.RemoveAt(i);
                    i--;
                }
            }

            oldKb = kb;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            for(int i = 0; i < missiles.Count; i++)
            {
                missiles[i].Draw(spriteBatch);
            }

            for(int i = 0; i < activeExplosions.Count; i++)
            {
                activeExplosions[i].Draw(spriteBatch, explosionTexture);
            }

            for(int i = 0; i < shrinkingExplosions.Count; i++)
            {
                shrinkingExplosions[i].Draw(spriteBatch, explosionTexture);
            }
            //bases
            //for (int i = 0; i < missilePos.Length; i++)
            //{
            //    spriteBatch.Draw(missileBase, missilePos[i], Color.White);
            //}
            //spriteBatch.Draw(L, land1, Color.Gold);
            //spriteBatch.Draw(L, land2, Color.Gold);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
