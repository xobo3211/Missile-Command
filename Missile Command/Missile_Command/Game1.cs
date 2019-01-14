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

        List<Missile> missiles;
        List<Explosion> activeExplosions;
        List<Explosion> shrinkingExplosions;

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

            missiles = new List<Missile>(20);
            activeExplosions = new List<Explosion>(20);
            shrinkingExplosions = new List<Explosion>(20);

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

            explosionTexture = Content.Load<Texture2D>("EFX/efx_explosion_b_0001");
            
            missiles.Add(new Missile(Content.Load<Texture2D>("2D/missile_small"), new Vector2(0, 0), new Vector2(2, 2), new Circle(new Vector2(300, 300), 3)));
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
            
            for(int i = 0; i < missiles.Count; i++)
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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
