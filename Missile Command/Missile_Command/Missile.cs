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

    public class Missile
    {
        public Vector2 position, velocity;

        public Circle endPos;                   //Hitbox to detect when missile reaches the end of it's travel. When missile enters said hitbox, it detonates

        static Texture2D texture = Useful.getTexture("missile_small");

        float rotation;
        int missileWidth = 3;                       //Width of missile
        int missileLength = 5;                      //Length of Missile

        public bool willExplode                 //Returns whether or not it is time for the missile to detonate
        {
            get
            {
                return endPos.Contains(position);       //If the missile's position is within the endPos circle, return true
            }
        }

        public Missile(Texture2D texture, Vector2 startPos, Vector2 velocity, Circle endPos)
        {
            position = startPos;
            this.velocity = velocity;

            this.endPos = endPos;

            rotation = (float)Math.Atan((double)(endPos.center.Y - startPos.Y / endPos.center.X - startPos.X));
        }

        public Missile(Texture2D texture, Vector2 startPos, float velocity, Vector2 endPos)
        {
            position = startPos;

            if (startPos.X > endPos.X)
                rotation = (float)Math.Atan((double)((endPos.Y - startPos.Y) / (endPos.X - startPos.X)));

            else rotation = (float)Math.Atan((double)(startPos.Y - endPos.Y) / (startPos.X - endPos.X));

            this.velocity = new Vector2(velocity * (float)Math.Cos(rotation), velocity * (float)Math.Sin(rotation));

            if(startPos.X > endPos.X)
            {
                this.velocity *= -1;
            }


            if (startPos.X > endPos.X)
            {
                rotation -= (float)Math.PI / 2;
            }
            else rotation += (float)Math.PI / 2;

            this.endPos = new Circle(endPos, velocity);
        }

        public void Update()
        {
            position += velocity;

        }

        public Explosion Detonate()
        {
            return new Explosion(position);
        }

        public Explosion Detonate(float explosionStartRadius)
        {
            return new Explosion(position, explosionStartRadius);
        }

        public void Draw(SpriteBatch b)
        {
            b.Draw(texture, new Rectangle((int)position.X - missileWidth / 2, (int)position.Y - missileLength / 2, missileWidth, missileLength), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
