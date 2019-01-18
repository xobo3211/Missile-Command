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
using SureDroid;

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
        Sprite curser;
        Text pointcounter;

        List<Missile> playerMissiles;
        List<Missile> enemyMissiles;

        List<Explosion> expandingExplosions;
        List<Explosion> shrinkingExplosions;

        MouseState m;
        KeyboardState oldKb;

        //Base Implementation
        Rectangle[] missilePos;
        Texture2D missileBase, L;
        Rectangle land1, land2;

        Rectangle[] basePos;

        Circle[] baseHitboxes;

        bool[] basesDisabled;
        bool[] citiesDestroyed;

        int[] playerMissilesLeft;

        int minFiringHeight = 100;           //Height in pixels above the bottom of screen that you can begin firing playerMissiles from



        public Game1()
        {
            Useful.set(this);

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500 * 2;
            graphics.PreferredBackBufferHeight = 500 * 2;
            
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

            curser = new Sprite();
            curser.setScale(.5);
            pointcounter = new Text("Points: " + Global.points, 10, 10);
            pointcounter.color = Color.White;

            //Creates empty lists to hold playerMissiles and explosions
            playerMissiles = new List<Missile>(20);
            enemyMissiles = new List<Missile>(20);
            expandingExplosions = new List<Explosion>(20);
            shrinkingExplosions = new List<Explosion>(20);

            //Controls
            m = Mouse.GetState();
            oldKb = Keyboard.GetState();

            //bases
            int framsX = GraphicsDevice.Viewport.Width;
            int framsY = GraphicsDevice.Viewport.Height;

            basePos = new Rectangle[3];

            basePos[0] = new Rectangle((int)Global.leftBasePosition.X - Global.baseWidth / 2, (int)Global.leftBasePosition.Y - Global.baseHeight / 2, Global.baseWidth, Global.baseHeight);
            basePos[1] = new Rectangle((int)Global.middleBasePosition.X - Global.baseWidth / 2, (int)Global.middleBasePosition.Y - Global.baseHeight / 2, Global.baseWidth, Global.baseHeight);
            basePos[2] = new Rectangle((int)Global.rightBasePosition.X - Global.baseWidth / 2, (int)Global.rightBasePosition.Y - Global.baseHeight / 2, Global.baseWidth, Global.baseHeight);


            basesDisabled = new bool[3] { false, false, false };                                                  //Initializes the bases as not being disabled
            citiesDestroyed = new bool[6] { false, false, false, false, false, false } ;                          //Initializes the cities as not having been destroyed
            playerMissilesLeft = new int[3] { 10, 10, 10 };                                                             //Initializes all bases to start with 10 playerMissiles

            float baseHitboxSize = 50f;                                                                           //Radius of circular base hitbox

            baseHitboxes = new Circle[3];
            baseHitboxes[0] = new Circle(Global.leftBasePosition, baseHitboxSize);
            baseHitboxes[1] = new Circle(Global.middleBasePosition, baseHitboxSize);
            baseHitboxes[2] = new Circle(Global.rightBasePosition, baseHitboxSize);

            Global.level = 1;
            Global.points = 0;
            Global.enemyMissilesLeft = 10;
            Global.enemyFireTimer = 0;


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
            //Explosion Texture
            explosionTexture = Content.Load<Texture2D>("EFX/efx_explosion_b_0001");

            //Base Texture
            spriteBatch = new SpriteBatch(GraphicsDevice);
            missileBase = Content.Load<Texture2D>("silo");
            L = Content.Load<Texture2D>("2D/missile_small");

            Missile.texture = Content.Load<Texture2D>("2D/missile_small");

            Text.setDefaultFont("font");
            curser.addTexture("curser");
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

            curser.setPos(m.X-curser.getWidth()/2, m.Y-curser.getHeight()/2);

            ///////////////////////////////////////////////     Firing Logic

            if (m.Y < GraphicsDevice.Viewport.Height - minFiringHeight)                                         //Sets minimum height for playerMissiles to be fired from to prevent our playerMissiles from destroying our own bases
            {

                if (kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) && playerMissilesLeft[0] > 0 && !basesDisabled[0])
                {
                    playerMissiles.Add(new Missile(Global.leftBasePosition, Global.slowPlayerMissileSpeed, new Vector2(m.X, m.Y)));
                    playerMissilesLeft[0]--;
                }

                if (kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) && playerMissilesLeft[1] > 0 && !basesDisabled[1])
                {
                    playerMissiles.Add(new Missile(Global.middleBasePosition, Global.fastPlayerMissileSpeed, new Vector2(m.X, m.Y)));
                    playerMissilesLeft[1]--;
                }

                if (kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D) && playerMissilesLeft[2] > 0 && !basesDisabled[2])
                {
                    playerMissiles.Add(new Missile(Global.rightBasePosition, Global.slowPlayerMissileSpeed, new Vector2(m.X, m.Y)));
                    playerMissilesLeft[2]--;
                }

            }

            ///////////////////////////////////////////////     Enemy Logic

            //Place enemy AI logic here

            if(Global.enemyMissilesLeft > 0 && Global.enemyFireTimer <= 0)
            {
                Random rn = new Random();

                int missilesFired = rn.Next(4) + 1;
                Global.enemyMissilesLeft -= missilesFired;

                for(int i = 0; i < missilesFired; i++)
                {
                    enemyMissiles.Add(new Missile(new Vector2(rn.Next(GraphicsDevice.Viewport.Width), 0), Global.enemyMissileSpeed, baseHitboxes[rn.Next(3)].center));
                }

                Global.enemyFireTimer = rn.Next(240) + 120;
            }




            for(int i = 0; i < baseHitboxes.Length; i++)                        //Detects whether any explosions are destroying any bases
            {
                if(basesDisabled[i] == false)                                   //Ignore collision on any bases already destroyed
                {

                    for (int a = 0; a < expandingExplosions.Count; a++)                //Compares base hitbox to all currently expanding explosions
                    {

                        if(baseHitboxes[i].Intersects(expandingExplosions[a].hitbox))
                        {
                            basesDisabled[i] = true;
                            playerMissilesLeft[i] = 0;
                            break;
                        }
                    }

                    for(int a = 0; a < shrinkingExplosions.Count; a++)                //Compares base hitbox to all currently shrinking explosions
                    {
                        if(baseHitboxes[i].Intersects(shrinkingExplosions[a].hitbox))
                        {
                            basesDisabled[i] = true;
                            playerMissilesLeft[i] = 0;
                            break;
                        }
                    }
                }
            }



            ///////////////////////////////////////////////     Update Logic

            for (int i = 0; i < playerMissiles.Count; i++)
            {
                playerMissiles[i].Update();
                if(playerMissiles[i].willExplode)
                {
                    expandingExplosions.Add(playerMissiles[i].Detonate());
                    playerMissiles.RemoveAt(i);
                    i--;
                }
            }

            try
            {
                for (int i = enemyMissiles.Count-1; i >= 0; i--)
                {
                    enemyMissiles[i].Update();
                    if (enemyMissiles[i].willExplode)                                           //Check if missile has reached the point it was aimed at
                    {
                        expandingExplosions.Add(enemyMissiles[i].Detonate());                   //If so, detonate it and add it to the explosion list
                        enemyMissiles.RemoveAt(i);
                    }
                    else                                                                        //If not, compare the missile to every one of the explosions
                    {
                        for (int a = 0; a < expandingExplosions.Count; a++)
                        {
                            if (expandingExplosions[a].hitbox.Contains(enemyMissiles[i].position))  //Compare to expanding explosions
                            {
                                expandingExplosions.Add(enemyMissiles[i].Detonate());
                                enemyMissiles.RemoveAt(i);
                                break;
                            }
                        }

                        for (int a = 0; a < shrinkingExplosions.Count; a++)
                        {
                            if (shrinkingExplosions[a].hitbox.Contains(enemyMissiles[i].position))  //Compare to shrinking explosions
                            {
                                expandingExplosions.Add(enemyMissiles[i].Detonate());
                                enemyMissiles.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                Console.WriteLine("Missile removal error");                 //Try-catch needed for draw errors caused by multithreading
            }

            for(int i = 0; i < expandingExplosions.Count; i++)
            {
                expandingExplosions[i].Update();

                if(expandingExplosions[i].finishedExpanding)
                {
                    shrinkingExplosions.Add(expandingExplosions[i]);
                    expandingExplosions.RemoveAt(i);
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

            Global.enemyFireTimer--;

            ////////// POINT AND LEVEL SYSTEM
            if(enemyMissiles.Count == 0 && Global.enemyMissilesLeft <= 0)
            {
                for(int i = 0; i < playerMissilesLeft.Length; i++)                  //Check every silo                                       
                {
                    Global.points += (5 * Global.level * playerMissilesLeft[i]);    //Add points for every missile left
                }

                for(int i = 0; i < citiesDestroyed.Length; i++)                     //Check every city
                {
                    if(!citiesDestroyed[i])                                         //If the city is destroyed...
                    {
                        Global.points += (200 * Global.level);                      //...Gain 200 * level points
                    }
                }

                for(int i = 0; i < basesDisabled.Length; i++)                       //Rebuild the bases and refill them
                {
                    basesDisabled[i] = false;
                    playerMissilesLeft[i] = 10;
                }
                pointcounter.content = "Points: " + Global.points;
                Global.level++;

                Global.enemyMissilesLeft = 10 + (Global.level * 2);
                Global.enemyMissileSpeed += 0.2f;
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

            Useful.drawAll(spriteBatch);

            for(int i = 0; i < playerMissiles.Count; i++)
            {
                playerMissiles[i].Draw(spriteBatch);
            }

            for(int i = 0; i < enemyMissiles.Count; i++)
            {
                enemyMissiles[i].Draw(spriteBatch);
            }

            for(int i = 0; i < expandingExplosions.Count; i++)
            {
                expandingExplosions[i].Draw(spriteBatch, explosionTexture);
            }

            for(int i = 0; i < shrinkingExplosions.Count; i++)
            {
                shrinkingExplosions[i].Draw(spriteBatch, explosionTexture);
            }
            //bases

            for (int i = 0; i < basePos.Length; i++)
            {
                spriteBatch.Draw(missileBase, basePos[i], Color.White);
            }

            //spriteBatch.Draw(L, land1, Color.Gold);
            //spriteBatch.Draw(L, land2, Color.Gold);

            //points
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
