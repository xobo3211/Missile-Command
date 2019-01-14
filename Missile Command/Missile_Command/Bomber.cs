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
    public class Bomber
    {
        public int points;
        public int speed;
        public int x;
        public int y;
        public int level;
        public Bomber(Texture2D t, int l)
        {
            level = l;

        }
    }

}
