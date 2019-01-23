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
        public const float slowPlayerMissileSpeed = 4f;                                     //Stores outer missile base missile speed
        public const float fastPlayerMissileSpeed = 8f;                                     //Stores middle missile base speed
        public static float enemyMissileSpeed = 1f;

        public static int level;
        public static int points;
        public static int enemyMissilesLeft;                                                //Counts number of missiles enemy has left to fire
        public static int enemyFireTimer;                                                   //Counts time until enemy fiers again
        public static int baseWidth = 60, baseHeight = 90;                                  //Controls missile base dimensions
        public static int cityWidth = 60, cityHeight = 40;                                  //Controls city dimensions
        public static int satelliteWidth = 50, satelliteHeight = 50;
        public static int missileWidth = 5, missileLength = 8;                              //Controls missile dimensions
        public static int missileTrailWidth = 1;
        public static int pointsToNextCity = 10000;                                         //Holds the amount of points that need to be earned to get another city

        public static HashSet<Vector2> destroyedTargets = new HashSet<Vector2>();           //Contains all currently destroyed city/base positions. Used to keep missiles from being shot at already destroyed areas

        public static readonly Vector2 leftBasePosition = new Vector2(Useful.getWWidth() * 0.05f, Useful.getWHeight() - baseHeight / 2);
        public static readonly Vector2 middleBasePosition = new Vector2(Useful.getWWidth() * 0.5f, Useful.getWHeight() - baseHeight / 2);
        public static readonly Vector2 rightBasePosition = new Vector2(Useful.getWWidth() * 0.95f, Useful.getWHeight() - baseHeight / 2);

        public static readonly Vector2[] cityPositions = new Vector2[]                 //Stores all positions for cities
        {
            new Vector2(Useful.getWWidth() * 0.1625f, Useful.getWHeight() - cityHeight / 2),
            new Vector2(Useful.getWWidth() * 0.275f, Useful.getWHeight() - cityHeight / 2),
            new Vector2(Useful.getWWidth() * 0.3875f, Useful.getWHeight() - cityHeight / 2),
            new Vector2(Useful.getWWidth() * 0.6125f, Useful.getWHeight() - cityHeight / 2),
            new Vector2(Useful.getWWidth() * 0.725f, Useful.getWHeight() - cityHeight / 2),
            new Vector2(Useful.getWWidth() * 0.8375f, Useful.getWHeight() - cityHeight / 2),
        };

        public static readonly Vector2[] targets = new Vector2[]                            //Stores all potential targets for missiles
        {
            leftBasePosition, cityPositions[0], cityPositions[1], cityPositions[2],
            middleBasePosition, cityPositions[3], cityPositions[4], cityPositions[5], rightBasePosition
        };


    }
}
