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
    public static class Global
    {
        public const float slowPlayerMissileSpeed = 3f;
        public const float fastPlayerMissileSpeed = 6f;
        public const float enemyMissileSpeed = 2f;

        public static int level;
        public static int points;
        public static int enemyMissilesLeft;
        public static int enemyFireTimer;


        public static readonly Vector2 leftBasePosition = new Vector2(Useful.getWWidth() * 0.15f, Useful.getWHeight() - 50);
        public static readonly Vector2 middleBasePosition = new Vector2(Useful.getWWidth() * 0.5f, Useful.getWHeight() - 50);
        public static readonly Vector2 rightBasePosition = new Vector2(Useful.getWWidth() * 0.85f, Useful.getWHeight() - 50);


    }
}
