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


public class Circle
{
    public Vector2 center { get; set; }
    public float radius { get; set; }
    public float diameter
    {
        get
        {
            return 2 * radius;
        }
    } 

	public Circle(Vector2 cen, float rad)
	{
        center = cen;
        radius = rad;
	}

    public bool Contains(Vector2 point)
    {
        return (center - point).Length() <= radius;
    }

    public bool Intersects(Circle other)
    {
        return (other.center - center).Length() < (other.radius - radius);
    }
}
