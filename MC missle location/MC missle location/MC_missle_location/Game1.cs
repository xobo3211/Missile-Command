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

namespace MC_missle_location
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle[] missilePos;
        Texture2D missileBase,L;
        Rectangle land1, land2;
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
            missilePos = new Rectangle[3];
            int framsX = GraphicsDevice.Viewport.Width;
            int framsY = GraphicsDevice.Viewport.Height;
            missilePos[0] = new Rectangle(0, framsY - 100, 125, 100);
            missilePos[1] = new Rectangle((framsX / 2) -100, framsY - 100, 125, 100);
            missilePos[2] = new Rectangle(framsX - 125, framsY - 100, 125, 100);
            land1 = new Rectangle(missilePos[0].X + missilePos[0].Width, missilePos[0].Y + (int)(missilePos[0].Width / 5),
                        Distance(missilePos[0], missilePos[1])- missilePos[0].Width, 100);
            land2 = new Rectangle(missilePos[1].X + missilePos[1].Width, missilePos[1].Y + (int)missilePos[1].Width / 5, 
                Distance(missilePos[1], missilePos[2]) - missilePos[1].Width,100);
            
                                

            base.Initialize();
        }

        public int Distance(Rectangle rect1, Rectangle rect2)
        {
            double dis = Math.Sqrt(Math.Pow(rect2.X - rect1.X, 2) + Math.Pow(rect2.Y - rect1.Y, 2));
            return (int) dis;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            missileBase = Content.Load<Texture2D>("missle base");
            L = Content.Load<Texture2D>("Star");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < missilePos.Length; i++)
            {
                spriteBatch.Draw(missileBase, missilePos[i], Color.White);
            }
            spriteBatch.Draw(L, land1, Color.Gold);
            spriteBatch.Draw(L, land2, Color.Gold);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
