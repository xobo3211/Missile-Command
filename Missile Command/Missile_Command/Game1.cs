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

        Random rand = new Random();

        public static List<Missile> playerMissiles;
        public static List<Missile> enemyMissiles;

        public static List<Explosion> expandingExplosions;
        public static List<Explosion> shrinkingExplosions;

        MouseState m;
        KeyboardState oldKb;

        //Base Implementation
        Texture2D missileBase, cityTexture;

        Rectangle[] basePos;

        Circle[] baseHitboxes, cityHitboxes;

        bool[] basesDisabled;
        bool[] citiesDestroyed;

        int[] playerMissilesLeft;

        int minFiringHeight = 100;           //Height in pixels above the bottom of screen that you can begin firing playerMissiles from


        bool startState = true, gameState, endState;


        public Game1()
        {
            Useful.set(this);

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 700;
            
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

            baseHitboxes = new Circle[3];                                                   //Creates hitboxes to detect if base is destroyed
            baseHitboxes[0] = new Circle(Global.leftBasePosition, baseHitboxSize);
            baseHitboxes[1] = new Circle(Global.middleBasePosition, baseHitboxSize);
            baseHitboxes[2] = new Circle(Global.rightBasePosition, baseHitboxSize);

            float cityHitboxSize = 30f;

            cityHitboxes = new Circle[6];                                                   //Creates hitboxes to detect if city is destroyed
            for(int i = 0; i < cityHitboxes.Length; i++)
            {
                cityHitboxes[i] = new Circle(Global.cityPositions[i], cityHitboxSize);
            }

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
            cityTexture = Content.Load<Texture2D>("2D/city02");

            Missile.texture = Content.Load<Texture2D>("2D/missile_small");
            Bomber.text = Useful.getTexture("plane");

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
            if(startState)
            {
                if(kb.IsKeyDown(Keys.Space))                                                                                    //Moves from start screen to gameplay if space is pressed
                {
                    startState = false;
                    gameState = true;
                }
            }
            else if (gameState)
            {

                if (m.Y < GraphicsDevice.Viewport.Height - minFiringHeight)                                         //Sets minimum height for playerMissiles to be fired from to prevent our playerMissiles from destroying our own bases
                {

                    if (kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) && playerMissilesLeft[0] > 0 && !basesDisabled[0])        //Fires from left base
                    {
                        Missile temp = new Missile(Global.leftBasePosition, Global.slowPlayerMissileSpeed, new Vector2(m.X, m.Y), Color.Blue, GraphicsDevice);
                        playerMissiles.Add(temp);
                        playerMissilesLeft[0]--;
                    }

                    if (kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) && playerMissilesLeft[1] > 0 && !basesDisabled[1])        //Fires from middle base
                    {
                        Missile temp = new Missile(Global.middleBasePosition, Global.fastPlayerMissileSpeed, new Vector2(m.X, m.Y), Color.Blue, GraphicsDevice);
                        playerMissiles.Add(temp);
                        playerMissilesLeft[1]--;
                    }

                    if (kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D) && playerMissilesLeft[2] > 0 && !basesDisabled[2])        //Fires from right base
                    {
                        Missile temp = new Missile(Global.rightBasePosition, Global.slowPlayerMissileSpeed, new Vector2(m.X, m.Y), Color.Blue, GraphicsDevice);
                        playerMissiles.Add(temp);
                        playerMissilesLeft[2]--;
                    }

                }

                ////////////////////////////////////////////////////////////////////////////////////     Enemy Logic

                //Place enemy AI logic here

            if(rand.Next(0,60) == 0)
            {
                new Bomber();
            }

            if(Global.enemyMissilesLeft > 0 && Global.enemyFireTimer <= 0)
            {
                Random rn = new Random();

                    int missilesFired = rn.Next(4) + 1;
                    Global.enemyMissilesLeft -= missilesFired;

                    for (int i = 0; i < missilesFired; i++)
                    {
                        Vector2 aimVec = Global.targets[rn.Next(Global.targets.Length)];

                        int escapeCase = 0;                                             //Used to prevent this from infinitely looping
                        while (Global.destroyedTargets.Contains(aimVec) && escapeCase < 100)    //Detects if target has already been destroyed, and if so, changes target
                        {
                            aimVec = Global.targets[rn.Next(Global.targets.Length)];
                            escapeCase++;
                        }
                        Missile temp = new Missile(new Vector2(rn.Next(GraphicsDevice.Viewport.Width), 0), Global.enemyMissileSpeed, aimVec, Color.Red, GraphicsDevice);
                        temp.clusterCalc();
                        enemyMissiles.Add(temp);
                    }

                    Global.enemyFireTimer = rn.Next(240) + 120;
                }




                for (int i = 0; i < baseHitboxes.Length; i++)                        //Detects whether any explosions are destroying any bases
                {
                    if (basesDisabled[i] == false)                                   //Ignore collision on any bases already destroyed
                    {

                        for (int a = 0; a < expandingExplosions.Count; a++)                //Compares base hitbox to all currently expanding explosions
                        {

                            if (baseHitboxes[i].Intersects(expandingExplosions[a].hitbox))
                            {
                                basesDisabled[i] = true;
                                playerMissilesLeft[i] = 0;
                                Global.destroyedTargets.Add(baseHitboxes[i].center);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < cityHitboxes.Length; i++)                        //Detects whether any explosions are destroying any cities
                {
                    if (citiesDestroyed[i] == false)                                   //Ignore collision on any cities already destroyed
                    {

                        for (int a = 0; a < expandingExplosions.Count; a++)                //Compares city hitbox to all currently expanding explosions
                        {

                            if (cityHitboxes[i].Intersects(expandingExplosions[a].hitbox))
                            {
                                citiesDestroyed[i] = true;
                                Global.destroyedTargets.Add(cityHitboxes[i].center);
                                break;
                            }
                        }
                    }
                }



                ////////////////////////////////////////////////////////////////////////////////////     Update Logic

                for (int i = 0; i < playerMissiles.Count; i++)
                {
                    playerMissiles[i].Update();
                    if (playerMissiles[i].willExplode)
                    {
                        expandingExplosions.Add(playerMissiles[i].Detonate());
                        playerMissiles.RemoveAt(i);
                        i--;
                    }
                }

            for(int i = Bomber.list.Count - 1; i >= 0; i--)
            {
                Bomber.list[i].update();
            }

            try
            {
                for (int i = enemyMissiles.Count-1; i >= 0; i--)
                {
                    enemyMissiles[i].Update();
                    if (enemyMissiles[i].willExplode && enemyMissiles[i].isClusterMissile == false)                                        
                    {
                        expandingExplosions.Add(enemyMissiles[i].Detonate());                   //If so, detonate it and add it to the explosion list
                        enemyMissiles.RemoveAt(i);
                    }
                    else if(enemyMissiles[i].willExplode && enemyMissiles[i].isClusterMissile)
                    {
                        Missile[] temp = enemyMissiles[i].ClusterSeparate(GraphicsDevice);

                            foreach (Missile m in temp)
                            {
                                enemyMissiles.Add(m);
                            }
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
                                    Global.points += 25;
                                    break;
                                }
                            }

                            for (int a = 0; a < shrinkingExplosions.Count; a++)
                            {
                                if (shrinkingExplosions[a].hitbox.Contains(enemyMissiles[i].position))  //Compare to shrinking explosions
                                {
                                    expandingExplosions.Add(enemyMissiles[i].Detonate());
                                    enemyMissiles.RemoveAt(i);
                                    Global.points += 25;
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

                for (int i = 0; i < expandingExplosions.Count; i++)
                {
                    expandingExplosions[i].Update();

                    if (expandingExplosions[i].finishedExpanding)
                    {
                        shrinkingExplosions.Add(expandingExplosions[i]);
                        expandingExplosions.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < shrinkingExplosions.Count; i++)
                {
                    shrinkingExplosions[i].Shrink();

                    if (shrinkingExplosions[i].finishedShrinking)
                    {
                        shrinkingExplosions.RemoveAt(i);
                        i--;
                    }
                }

                

                if(Global.pointsToNextCity <= 0)                                            //Code for adding back new cities when you reach a certain level of points
                {
                    Random rn = new Random();

                    int cityIndex = rn.Next(6), defaultCase = 0;
                    while(citiesDestroyed[cityIndex] == false && defaultCase < 100)
                    {
                        cityIndex = rn.Next(6);
                        defaultCase++;
                    }

                    citiesDestroyed[cityIndex] = false;
                    Global.pointsToNextCity = 10000;
                }

                //////////////////////////////////////////////////////////////////////////////////// POINT AND LEVEL SYSTEM
                if (enemyMissiles.Count == 0 && expandingExplosions.Count == 0 && shrinkingExplosions.Count == 0 && Global.enemyMissilesLeft <= 0)
                {
                    for (int i = 0; i < playerMissilesLeft.Length; i++)                  //Check every silo                                       
                    {
                        Global.points += (5 * Global.level * playerMissilesLeft[i]);    //Add points for every missile left
                        Global.pointsToNextCity -= (5 * Global.level * playerMissilesLeft[i]);
                    }

                    for (int i = 0; i < citiesDestroyed.Length; i++)                     //Check every city
                    {
                        if (!citiesDestroyed[i])                                         //If the city is destroyed...
                        {
                            Global.points += (200 * Global.level);                      //...Gain 200 * level points
                            Global.pointsToNextCity -= (200 * Global.level);
                        }
                    }

                    for (int i = 0; i < basesDisabled.Length; i++)                       //Rebuild the bases and refill them
                    {
                        basesDisabled[i] = false;
                        playerMissilesLeft[i] = 10;
                        Global.destroyedTargets.Remove(Global.leftBasePosition);
                        Global.destroyedTargets.Remove(Global.middleBasePosition);
                        Global.destroyedTargets.Remove(Global.rightBasePosition);
                    }
                    Global.level++;

                    Global.enemyMissilesLeft = 10 + (Global.level * 2);
                    Global.enemyMissileSpeed += 0.2f;

                    bool gameEnd = true;                                                //End logic
                        
                    for(int i = 0; i < citiesDestroyed.Length; i++)                     //Detect if any cities aren't destroyed. If so, game doesn't end
                    {
                        if(citiesDestroyed[i] == false)
                        {
                            gameEnd = false;
                            break;
                        }
                    }

                    if(gameEnd)                                                         //Changes game state to end state
                    {
                        gameState = false;
                        endState = true;
                    }
                }

                Global.enemyFireTimer--;
                pointcounter.content = "Points: " + Global.points;
            }
            else if(endState)
            {

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
            if(startState)
            {

            }
            else if (gameState)
            {

                Useful.drawAll(spriteBatch);

                for (int i = 0; i < playerMissiles.Count; i++)
                {
                    playerMissiles[i].Draw(spriteBatch);
                }

                for (int i = 0; i < enemyMissiles.Count; i++)
                {
                    enemyMissiles[i].Draw(spriteBatch);
                }

                for (int i = 0; i < expandingExplosions.Count; i++)
                {
                    expandingExplosions[i].Draw(spriteBatch, explosionTexture);
                }

                for (int i = 0; i < shrinkingExplosions.Count; i++)
                {
                    shrinkingExplosions[i].Draw(spriteBatch, explosionTexture);
                }

                for (int i = 0; i < basePos.Length; i++)
                {
                    spriteBatch.Draw(missileBase, basePos[i], Color.White);
                }

                for (int i = 0; i < Global.cityPositions.Length; i++)
                {
                    if (!citiesDestroyed[i])
                    {
                        Rectangle drawRect = new Rectangle((int)Global.cityPositions[i].X - Global.cityWidth / 2, (int)Global.cityPositions[i].Y - Global.cityHeight / 2,
                                                            Global.cityWidth, Global.cityHeight);
                        spriteBatch.Draw(cityTexture, drawRect, Color.White);
                    }
                }
            }
            else if(endState)
            {

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
