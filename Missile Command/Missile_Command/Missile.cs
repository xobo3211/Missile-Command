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

    public class Missile
    {
        public Vector2 position, velocity;

        public Circle endPos;                   //Hitbox to detect when missile reaches the end of it's travel. When missile enters said hitbox, it detonates

        public static Texture2D texture;

        float rotation;

        Line trail;                             //Stores line trail for the missile

        public bool isClusterMissile;                  //Detects if missile will split into a cluster
        int clusterCount;                       //Stores number of missiles it will separate into

        public bool willExplode                 //Returns whether or not it is time for the missile to detonate
        {
            get
            {
                return endPos.Contains(position);       //If the missile's position is within the endPos circle, return true
            }
        }

        public Missile( Vector2 startPos, Vector2 velocity, Circle endPos, Color trailColor, GraphicsDevice g)
        {
            position = startPos;
            this.velocity = velocity;

            this.endPos = endPos;
            
            rotation = (float)Math.Atan((double)(endPos.center.Y - startPos.Y / endPos.center.X - startPos.X));

            createTrail(g, trailColor);
        }

        public Missile(Vector2 startPos, float velocity, Vector2 endPos, Color trailColor, GraphicsDevice g)
        {
            position = startPos;


            ////////Calculations for rotation so that missile can be drawn rotated correctly and aimed correctly
            if (startPos.X > endPos.X)
                rotation = (float)Math.Atan((double)((endPos.Y - startPos.Y) / (endPos.X - startPos.X)));

            else rotation = (float)Math.Atan((double)(startPos.Y - endPos.Y) / (startPos.X - endPos.X));


            ///////Calculations for aiming the missile correctly
            this.velocity = new Vector2(velocity * (float)Math.Cos(rotation), velocity * (float)Math.Sin(rotation));

            if(startPos.X > endPos.X)
            {
                this.velocity *= -1;            //Fixes issue where missile aims in the wrong direction if aimed too far to the left
            }


            if (startPos.X > endPos.X)          //Fixes rotation so that missile doesn't fly horizontally
            {
                rotation -= (float)Math.PI / 2;
            }
            else rotation += (float)Math.PI / 2;

            this.endPos = new Circle(endPos, velocity);     //Creates the end position circle for the missile to collide with

            createTrail(g, trailColor);                     //Creates the trail for the missile
        }

        public void Update()
        {
            position += velocity;                                     //Alters the missile's position according to it's velocity

            trail.setEndPoint(position);                              //Sets the line for the trail to have it's end point at the missile's new position

        }

        private void createTrail(GraphicsDevice g, Color c)           //This MUST be called immediately after the missile is created
        {
            trail = new Line(position, position, Global.missileTrailWidth, c, g);
        }

        public Explosion Detonate()                                 //Creates an Explosion object based on the missile's current position
        {
            return new Explosion(position);
        }

        public Explosion Detonate(float explosionStartRadius)       //Creates an Explosion object with a custom starting radius
        {
            return new Explosion(position, explosionStartRadius);
        }

        public Missile[] ClusterSeparate(GraphicsDevice g)          //Creates an array of missiles from a separating cluster missile
        {
            Missile[] newMissiles = new Missile[clusterCount];

            Random rn = new Random();

            for (int i = 0; i < clusterCount; i++)
            {
                Vector2 aimVec = Global.targets[rn.Next(Global.targets.Length)];

                int escapeCase = 0;                                             //Used to prevent this from infinitely looping
                while (Global.destroyedTargets.Contains(aimVec) && escapeCase < 100)    //Detects if target has already been destroyed, and if so, changes target
                {
                    aimVec = Global.targets[rn.Next(Global.targets.Length)];
                    escapeCase++;
                }
                newMissiles[i] = new Missile(position, Global.enemyMissileSpeed, aimVec, Color.Red, g);
                newMissiles[i].isClusterMissile = false;                        //Guarantees new missiles created won't cluster
            }

            return newMissiles;
        }

        public void Draw(SpriteBatch b)
        {
            if (trail != null)              //Makes sure trail isn't null before drawing it
                trail.Draw(b);              //Draws trail first so that trail is drawn behind the missile

            b.Draw(texture, new Rectangle((int)position.X, (int)position.Y, Global.missileWidth, Global.missileLength), null, Color.White, rotation,
                        new Vector2(Global.missileWidth, Global.missileLength), SpriteEffects.None, 0);
        }

        public void clusterCalc()                                   //Calculates whether missile will be a cluster missile and handles logic related to that
        {                                                           //IMPORTANT: Must be called after the creation of every enemy missile.
            Random rn = new Random();

            float clusterChance = 0.1f + ((Global.level - 1) * 0.02f);

            int clusterChanceCalc = (int)(clusterChance * 100);

            isClusterMissile = clusterChanceCalc >= rn.Next(101);   //Calculates chance and determines if it is a cluster missile

            if (isClusterMissile)                                    //If missile is a cluster missile, it will cluster into 2-level/2 missiles
            {
                clusterCount = rn.Next(Global.level / 2) + 2;

                float distance = (position - endPos.center).Length();                       //Changes endPos so it will split at endPos
                int steps = (int)(distance / velocity.Length());
                float distanceMod = rn.Next(40) + 20;
                steps = (int)(steps * distanceMod / 100);

                endPos.center = position + (steps * velocity);
            }
        }
    }
}
