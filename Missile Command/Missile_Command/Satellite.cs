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
    class Satellite
    {
        Vector2 position;
        static float speed = 2f;

        int fireTimer;

        public bool willFire
        {
            get
            {
                return fireTimer <= 0;
            }
        }

        public Satellite()
        {
            Random rn = new Random();

            position = new Vector2(-30, rn.Next(20) + 20);

            fireTimer = rn.Next(60) + 30;           //Satellite fires every half second to one and a half seconds
        }

        public void Update()
        {
            position.X += speed;
            fireTimer--;
        }

        public Missile Fire(Vector2 target, GraphicsDevice g)
        {
            return new Missile(position, Global.enemyMissileSpeed, target, Color.Red, g);
        }

        public void Draw(Texture2D texture, SpriteBatch b)
        {
            b.Draw(texture, new Rectangle((int)position.X - Global.satelliteWidth / 2, (int)position.Y - Global.satelliteHeight / 2, Global.satelliteWidth, Global.satelliteHeight), Color.White);
        }
    }
}
