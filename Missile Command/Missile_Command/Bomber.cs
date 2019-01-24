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
    public class Bomber : Enemy
    {
        public Bomber(Texture2D texture) : base()
        {
            speed = 0.9f;

            hitbox = new Circle(new Vector2(-30, 110), Global.bomberWidth / 3);

            this.texture = texture;

            Random rn = new Random();

            fireTimer = rn.Next(180) + 180;
        }

        public override Missile Fire(Vector2 target, GraphicsDevice g)
        {
            Random rn = new Random();
            fireTimer = rn.Next(180) + 180;
            return new Missile(hitbox.center, Global.enemyMissileSpeed, target, Color.Red, g);
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw(texture, new Rectangle((int)hitbox.center.X - Global.bomberWidth / 2, (int)hitbox.center.Y - Global.bomberHeight / 2, Global.bomberWidth, Global.bomberHeight), Color.White);
        }
    }

}
