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


public class Line
{
    Vector2 startPoint;
    Vector2 endPoint;
    int width;

    Color c;

    Texture2D texture;

    private double rotation     //Returns rotation of the line for drawing
    {
        get
        {
            return
                startPoint.X < endPoint.X ?
                    Math.Atan((endPoint - startPoint).Y / (endPoint - startPoint).X)
                    : Math.Atan((endPoint - startPoint).Y / (endPoint - startPoint).X) + Math.PI;

        }
    }


    public Line(Vector2 startPoint, Vector2 endPoint,int width, Color c, GraphicsDevice g)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.width = width;
        this.c = c;

        texture = new Texture2D(g, 1, 1);
        texture.SetData<Color>(new Color[] { Color.White } );     //Creates 1x1 texture of white 
    }

    public void setOrigin(Vector2 newStart)             //Changes start position for line drawing
    {
        startPoint = newStart;
    }
        
    public void setEndPoint(Vector2 newEnd)             //Changes end position for line drawing
    {
        endPoint = newEnd;
    }

    public void setColor(Color c)
    {
        this.c = c;
    }

    public void Draw(SpriteBatch b)
    {
        Rectangle drawRect = new Rectangle((int)Math.Abs(startPoint.X), (int)Math.Abs(startPoint.Y),
                (int)(startPoint - endPoint).Length(), width);

        b.Draw(texture, drawRect, null, c, (float)rotation, new Vector2(0, 0), SpriteEffects.None, 0);
    }
}