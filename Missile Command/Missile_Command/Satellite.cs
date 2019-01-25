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
    public class Satellite : Enemy
    {
        public Satellite(Texture2D texture) : base()
        {
            speed = 1.3f;

            hitbox = new Circle(new Vector2(-30, 60), Global.satelliteWidth/2);

            this.texture = texture;

            Random rn = new Random();

            fireTimer = rn.Next(120) + 120;
        }

        public override Missile Fire(Vector2 target, GraphicsDevice g)
        {
            Random rn = new Random();
            fireTimer = rn.Next(120) + 120;
            return new Missile(hitbox.center, Global.enemyMissileSpeed, target, Color.Red, g);
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw(texture, new Rectangle((int)hitbox.center.X - Global.satelliteWidth / 2, (int)hitbox.center.Y - Global.satelliteHeight / 2, Global.satelliteWidth, Global.satelliteHeight), Color.White);
        }
    }
}
