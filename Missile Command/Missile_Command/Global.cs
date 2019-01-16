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
        public const float slowMissileSpeed = 2f;
        public const float fastMissileSpeed = 5f;

        public static readonly Vector2 leftBasePosition = new Vector2(Useful.getWWidth() * 0.15f, Useful.getWHeight() - 50);
        public static readonly Vector2 middleBasePosition = new Vector2(Useful.getWWidth() * 0.5f, Useful.getWHeight() - 50);
        public static readonly Vector2 rightBasePosition = new Vector2(Useful.getWWidth() * 0.85f, Useful.getWHeight() - 50);


    }
}
