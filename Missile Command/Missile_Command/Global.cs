﻿using System;
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
        public const float slowPlayerMissileSpeed = 4f;                                     //Stores outer missile base missile speed
        public const float fastPlayerMissileSpeed = 8f;                                     //Stores middle missile base speed
        public static float enemyMissileSpeed = 1f;

        public static int level;
        public static int points;
        public static int enemyMissilesLeft;                                                //Counts number of missiles enemy has left to fire
        public static int enemyFireTimer;                                                   //Counts time until enemy fiers again
        public static int baseWidth = 70, baseHeight = 80;                                  //Controls missile base dimensions
        public static int missileWidth = 5, missileLength = 8;                              //Controls missile dimensions
        public static int missileTrailWidth = 1;


        public static readonly Vector2 leftBasePosition = new Vector2(Useful.getWWidth() * 0.1f, Useful.getWHeight() - baseHeight / 2);
        public static readonly Vector2 middleBasePosition = new Vector2(Useful.getWWidth() * 0.5f, Useful.getWHeight() - baseHeight / 2);
        public static readonly Vector2 rightBasePosition = new Vector2(Useful.getWWidth() * 0.9f, Useful.getWHeight() - baseHeight / 2);


    }
}
