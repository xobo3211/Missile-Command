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
    public class Explosion
    {

        Circle hitbox;

        float defaultExplosionRadius = 5f, maxExplosionRadius = 25f;

        public Explosion(Vector2 pos)
        {
            hitbox = new Circle(pos, defaultExplosionRadius);
        }

        public Explosion(Vector2 pos, float radius)
        {
            hitbox = new Circle(pos, radius);
        }

        public void Update()
        {
            hitbox.radius++;
        }

        public void Draw(SpriteBatch b, Texture2D texture)
        {
            Rectangle drawRect = new Rectangle((int)(hitbox.center.X - hitbox.radius), (int)(hitbox.center.Y - hitbox.radius), (int)hitbox.diameter, (int)hitbox.diameter);
            b.Draw(texture, drawRect, Color.White);
        }
    }
}
