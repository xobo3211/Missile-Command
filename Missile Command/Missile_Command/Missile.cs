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

        Texture2D texture;

        float rotation, missileSize;

        public Missile(Vector2 startPos, Vector2 velocity, Circle endPos)
        {
            position = startPos;
            this.velocity = velocity;

            rotation = (float)Math.Atan((double)(endPos.center.Y - startPos.Y / endPos.center.X - startPos.X));
        }

        public void Update()
        {
            position += velocity;

        }

        public Circle Detonate(float explosionStartRadius)
        {
            return new Circle(position, explosionStartRadius);
        }

        public void Draw(SpriteBatch b, Texture texture)
        {
            b.Draw(texture, new Rectangle(position.X - missileSize/2))
        }
    }
}
