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
    public class Bomber : Sprite
    {
        public static List<Bomber> list = new List<Bomber>();
        static Random rand = new Random();
        public static Texture2D text;
        int speed = 1;
        public Bomber() : base(-10, rand.Next(0, Useful.getWHeight() - 200))
        {
            addTexture(text);
            list.Add(this);
            setScale(.1);
            centerOrigin();
        }
        
        public new void update()
        {
            translate(speed,0);
            int x = (int) getPos().X;
            foreach(Vector2 vec in Global.targets)
            {
                if((int) vec.X == x)
                {
                    if(rand.Next(0,3)==0)
                        Game1.enemyMissiles.Add(new Missile(getPos(), Global.enemyMissileSpeed, vec, Color.Red, Useful.game.GraphicsDevice));           
                }
            }
            for (int a = 0; a < Game1.expandingExplosions.Count; a++)
            {
                if (Game1.expandingExplosions[a].hitbox.Contains(getPos()))  //Compare to expanding explosions
                {
                    delete();
                    return;
                }
            }

            for (int a = 0; a < Game1.shrinkingExplosions.Count; a++)
            {
                if (Game1.shrinkingExplosions[a].hitbox.Contains(getPos()))  //Compare to shrinking explosions
                {
                    delete();
                    return;
                }
            }
            if(x > Useful.getWWidth())
            {
                delete();
                return;
            }

        }

        public new void delete()
        {
            base.delete();
            list.Remove(this);
        }
    }

}
