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
    abstract public class Enemy
    {
        public static float speed;
        public Circle hitbox;

        public Texture2D texture;

        public int fireTimer;

        public bool willFire
        {
            get
            {
                return fireTimer <= 0;
            }
        }

        public void Update()
        {
            hitbox.center.X += speed;
            fireTimer--;
        }

        public Explosion Detonate()                                 //Creates an Explosion object based on the enemy's current position
        {
            return new Explosion(hitbox.center);
        }

        abstract public Missile Fire(Vector2 target, GraphicsDevice g);

        abstract public void Draw(SpriteBatch b);
    }
}
