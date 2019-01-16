﻿
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
using System.Diagnostics;

namespace SureDroid
{
    /// <summary>
    /// Custom Sprite Class - Created By Rithvik Senthilkumar | 
    /// Allows for easy handling of teextures, sprites, and animations. | 
    /// If you are not Rithvik and do not have permission from him, please dispose of this file immediately.
    /// </summary>
    public class Sprite
    {
        // -----------------------------------------------------------------
        // -------------------------- Variables ----------------------------
        // -----------------------------------------------------------------
        public static List<Sprite> sprites = new List<Sprite>();

        private Rectangle rectangle;
        private Vector2 pos, origin;
        private List<Texture2D> textures = new List<Texture2D>();
        private List<Size> sizes = new List<Size>();
        private List<List<Rectangle>> regions = new List<List<Rectangle>>();
        private int index = 0, regionIndex = 0;
        private float timer = 0, rotation = 0;
        private Boolean visible = true, animation = false, regionUse = false;
        private Double scale = 1.0;
        private float timeframe = 1 / 3f;
        private SpriteEffects effect = SpriteEffects.None;
        private Color color = Color.White;

        // -------------------------------------------------------------------
        // ------------------------- Constructors ----------------------------
        // -------------------------------------------------------------------

        /// <summary>
        /// Initializes the sprite to a provided x and y location.
        /// </summary>
        public Sprite(float x, float y)
        {
            if (Useful.game == null) throw new NullReferenceException("Please use Useful.set(Game game) before you use any class.");
            rectangle = new Rectangle();
            pos = new Vector2(x, y);
            origin = new Vector2(0, 0);
            updatePos();
            sprites.Add(this);
        }

        /// <summary>
        /// Initializes the sprite to a default (0, 0) position.
        /// </summary>
        public Sprite() : this(0, 0) { }

        /// <summary>
        /// Initialize the sprite class using with a provided position.
        /// </summary>
        /// <param name="pos">Position for the sprite in a vector2 format</param>
        public Sprite(Vector2 pos) : this(pos.X, pos.Y) { }

        // -----------------------------------------------------------------
        // ---------------------- GET/SET CODE -----------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Get the width of the sprite on the current index.
        /// </summary>
        public int getWidth()
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            //if (!regionUse) 
            return (int)((Double)sizes[index].width * scale);
            //else return (int)((Double)getRegion().Width * scale);
        }

        /// <summary>
        /// Get the height of the sprite on the current index.
        /// </summary>
        public int getHeight()
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            //if (!regionUse) 
            return (int)((Double)sizes[index].height * scale);
            //else return (int)((Double)getRegion().Height * scale);
        }

        /// <summary>
        /// Set the effect for a sprite.
        /// </summary>
        /// <param name="newEffect">An effect from the SpriteEffects class.</param>
        public void setEffect(SpriteEffects newEffect)
        {
            effect = newEffect;
        }

        /// <summary>
        /// Set the color of the sprite.
        /// </summary>
        /// <param name="color">A color from the color class.</param>
        public void setColor(Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Return if a sprites intersects with another sprite.
        /// </summary>
        /// <param name="other">The other sprite you are comparing for intersection with.</param>
        /// <returns></returns>
        public Boolean intersects(Sprite other)
        {
            return getRectangle().Intersects(other.getRectangle());
        }
        
        /// <summary>
        /// Gets the rectangle for the current sprite. No changes can be made.
        /// </summary>
        /// <returns>A rectangle, containing size and position.</returns>
        public Rectangle getRectangle()
        {
            updatePos();
            return rectangle;
        }

        /// <summary>
        /// Sets the order of this sprite to move behind or in front of a sprite.
        /// </summary>
        public void setOrder(int index)
        {
            sprites.Remove(this);
            sprites.Insert(index, this);
        }

        /// <summary>
        /// Sets the rotation for the sprite using degrees.
        /// </summary>
        public void setRotation(float d)
        {
            //if(d < 0 || d > 360) throw new ArgumentOutOfRangeException("Degrees is not within bounds.");
            rotation = MathHelper.ToRadians(d);
        }

        /// <summary>
        /// Rotates the sprite using degrees.
        /// </summary>
        public void rotate(float d)
        {
            //if (d < 0 || d > 360) Console.WriteLine("Degrees is not within bounds.");
            float degrees = MathHelper.ToDegrees(rotation);
            if (degrees + d < 0) degrees = (360 - Math.Abs(degrees + d));
            else if (degrees + d > 360) degrees = degrees + d - 360;
            else degrees += d;
            rotation = MathHelper.ToRadians(degrees);
        }

        /// <summary>
        /// Returns the rotation of the sprite in degrees.
        /// </summary>
        public float getRotation()
        {
            return MathHelper.ToDegrees(rotation);
        }

        /// <summary>
        /// Returns the rotation of the sprite in degrees. Useful for methods such as Math.Cos, Math.Sin, etc which requires radians.
        /// </summary>
        public float getRotationRadians()
        {
            return rotation;
        }

        /// <summary>
        /// Returns the index of the sprite.
        /// </summary>
        public int getIndex()
        {
            return index;
        }

        /// <summary>
        /// Sets the origin for the sprite.
        /// </summary>
        public void setOrigin(int x, int y)
        {
            if (x > getWidth() || x < 0 || y > getHeight() || y < 0) throw new ArgumentOutOfRangeException("The origin is out of bounds.");
            if (!regionUse)
            {
                origin.X = (int)(x / scale);
                origin.Y = (int)(y / scale);
            }
            else
            {
                origin.X = (int)((x + getRegion().X) / scale);
                origin.Y = (int)((y + getRegion().Y) / scale);
            }
        }

        /// <summary>
        /// Centers the sprite based on the current size. Only works once size is determined.
        /// </summary>
        public void centerOrigin()
        {
            setOrigin(getWidth() / 2, getHeight() / 2);
        }

        /// <summary>
        /// Enables or disables if the sprite is rendered.
        /// </summary>
        public void setVisible(Boolean value)
        {
            visible = value;
        }

        /// <summary>
        /// Sets the visibility for all sprites.
        /// </summary>
        public static void setAllVisible(Boolean value)
        {
            foreach (Sprite sprite in sprites) sprite.setVisible(value);
        }

        /// <summary>
        /// Sets the size of the sprite based off of the scale factor.
        /// </summary>
        public void setScale(Double factor)
        {
            scale = factor;
        }

        /// <summary>
        /// Sets the size of the sprite based off of the provided width and height.
        /// </summary>
        public void setSize(int width, int height)
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            sizes[index].set(width, height);
        }


        // -----------------------------------------------------------------
        // ---------------------- TEXTURE CODE -----------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Adds the displayable texture to the sprite and sets the size.
        /// </summary>
        public void addTexture(Texture2D texture)
        {
            textures.Add(texture);
            sizes.Add(new Size(texture.Width, texture.Height));
            regions.Add(new List<Rectangle>());
            rectangle.Width = texture.Width;
            rectangle.Height = texture.Height;
        }

        /// <summary>
        /// Sets the sprite's active texture to one specified by the provided index.
        /// </summary>
        public void setTexture(int index)
        {
            if (index > textures.Count - 1 || index < 0)
            {
                Console.WriteLine("Invalid Index");
                return;
            }
            this.index = index;
            regionIndex = 0;
        }

        /// <summary>
        /// Sets the sprite's active texture to the next one.
        /// </summary>
        public void nextTexture()
        {
            index++;
            if (index > textures.Count - 1) index = 0;
        }

        /// <summary>
        /// Sets the sprite's active texture to the previous one.
        /// </summary>
        public void previousTexture()
        {
            index--;
            if (index < 0) index = textures.Count - 1;
        }

        /// <summary>
        /// Gets the current texture based off the textureIndex.
        /// </summary>
        public Texture2D getTexture()
        {
            return textures[index];
        }

        /// <summary>
        /// Gets the current texture based off the provided index.
        /// </summary>
        public Texture2D getTexture(int index)
        {
            return textures[index];
        }

        /// <summary>
        /// Adds a texture to the sprite using a provided filename. You can only use this if you set the Content Manager in a previous method.
        /// </summary>
        public void addTexture(String fileName)
        {
            //if (contentManager == null) throw new NullReferenceException("You have not set the content manager yet. Please use the setLoader modethod to set the content manager before you run this method.");
            addTexture(Useful.game.Content.Load<Texture2D>(fileName));
            regions.Add(new List<Rectangle>());
        }


        // --------------------------------------------------------------------------
        // ---------------------------- REGION CODE ---------------------------------
        // --------------------------------------------------------------------------

        /// <summary>
        /// Define the rectangle for animation.
        /// </summary>
        public void defRegion(int x, int y, int width, int height)
        {
            getRegionList().Add(new Rectangle(x, y, width, height));
            sizes.Add(new Size(width, height));
        }

        /// <summary>
        /// Sets the current region for the sprite using a provided index.
        /// </summary>
        /// <param name="index">An integer within the amount of regions.</param>
        public void setRegion(int index)
        {
            if (index < 0 || index > getRegionList().Count()) throw new IndexOutOfRangeException("There is no region with this index.");
            regionIndex = index;
        }

        /// <summary>
        /// Call this before you start adding regions.
        /// </summary>
        /// <param name="value"></param>
        public void useRegion(Boolean value)
        {
            regionUse = value;
            sizes.Clear();
        }

        /// <summary>
        /// Sets the sprite's active region to the next one.
        /// </summary>
        public void nextRegion()
        {
            regionIndex++;
            if (regionIndex > regions[index].Count - 1) regionIndex = 0;
        }

        /// <summary>
        /// Sets the sprite's active region to the previous one.
        /// </summary>
        public void previousRegion()
        {
            regionIndex--;
            if (regionIndex < 0) regionIndex = regions[index].Count - 1;
        }

        /// <summary>
        /// Gets the region using a provided texture index and a provided regionIndex for the texture.
        /// </summary>
        public Rectangle getRegion(int index, int regionIndex)
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region using a provided texture index and the currently active regionIndex.
        /// </summary>
        public Rectangle getRegion(int index)
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region using the currently active texture index and the currently active regionIndex.
        /// </summary>
        public Rectangle getRegion()
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region list for the provided texture index.
        /// </summary>
        public List<Rectangle> getRegionList(int index)
        {
            return regions[index];
        }

        /// <summary>
        /// Gets the region list for the currently active region index.
        /// </summary>
        public List<Rectangle> getRegionList()
        {
            return regions[index];
        }

        // --------------------------------------------------------------------
        // ------------------------ ANIMATION CODE ----------------------------
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates the animation using the timer and comparing it to the timeframe.
        /// </summary>
        private void updateAnimation()
        {
            timer += 1 / 60f;
            if (timer >= timeframe)
            {
                if (!regionUse)
                    nextTexture();
                else nextRegion();
                timer = 0;
            }
        }

        /// <summary>
        /// Enables or disables the animation loop.
        /// </summary>
        public void setAnimation(Boolean value)
        {
            timer = index = 0;
            animation = value;
        }

        /// <summary>
        /// Set how fast you want each animation to loop per second.
        /// Default is one frame every three seconds.
        /// </summary>
        public void setTimeFrame(float value)
        {
            timeframe = value;
        }

        // -------------------------------------------------------------------------
        // --------------------------- POSITION CODE -------------------------------
        // -------------------------------------------------------------------------

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }

        /// <summary>
        /// Sets the position of the sprite.
        /// </summary>
        public void setPos(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
        }

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(Vector2 pos)
        {
            this.pos += pos;
        }

        /// <summary>
        /// Sets the position of the sprite.
        /// </summary>
        public void setPos(Vector2 pos)
        {
            this.pos = pos;
        }

        /// <summary>
        /// Returns the position of the sprite in a Vector2 format.
        /// </summary>
        public Vector2 getPos()
        {
            return pos;
        }

        /// <summary>
        /// Updates the position of the rectangle respective to the vector2 position variable.
        /// </summary>
        private void updatePos()
        {
            rectangle.X = (int)(pos.X);
            rectangle.Y = (int)(pos.Y);
        }

        // -----------------------------------------------------------------
        // ----------------------- USESPRITE CODE --------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Updates all the logic in the sprite classes.
        /// Required if you want to do animation.
        /// </summary>
        public void update()
        {
            if (animation) updateAnimation();
        }

        /// <summary>
        /// Draws the sprite if it is visable.
        /// </summary>
        public void draw(SpriteBatch batch)
        {
            if (!visible) return;
            updatePos();
            rectangle.Width = getWidth();
            rectangle.Height = getHeight();
            if(!regionUse) batch.Draw(getTexture(), rectangle, null, color, rotation, origin, effect, 0);
            else batch.Draw(getTexture(), rectangle, getRegion(), color, rotation, origin, effect, 0);
        }


        /// <summary>
        /// Draws all sprites created in order of creation using a provided spritebatch.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        { 
            foreach (Sprite sprite in sprites)
            {
                sprite.draw(batch);
            }
        }
        /*
        /// <summary>
        /// Draws all sprites created in order of creation using the gloabal spritebatch.
        /// </summary>
        public static void drawAll()
        {
            drawAll(Useful.spriteBatch);
        }
        */

        /// <summary>
        /// Updates all sprites created.
        /// </summary>
        public static void updateAll()
        {
            foreach(Sprite sprite in sprites)
            {
                sprite.update();
            }
        }

        /// <summary>
        /// Storage of size in integeger form.
        /// </summary>
        private class Size
        {
            public int width, height;
            public Size(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
            public void set(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }
    }

    /// <summary>
    /// Custom Texture Class - Created By Rithvik Senthilkumar | 
    /// Allows for easy handling of text. | 
    /// If you are not Rithvik and do not have permission from him, please dispose of this file immediately.
    /// </summary>
    public class Text
    {
        private static List<Text> texts = new List<Text>();
        public String content = "";
        public Vector2 pos = new Vector2(0, 0);
        private SpriteFont font;
        private static SpriteFont normal;
        public Boolean visable = true, useFont = false;
        public Color color = Color.Black;
        /// <summary>
        /// Sets the default font for all fonts.
        /// </summary>
        /// <param name="fontName"></param>
        public static void setDefaultFont(String fontName)
        {
            normal = Useful.game.Content.Load<SpriteFont>(fontName);
        }
        /// <summary>
        /// Create new Text Object with all values defaulted.
        /// </summary>
        public Text() {
            texts.Add(this);
        }
        
        /// <summary>
        /// Create a Text class with content provided and default position values.
        /// </summary>
        public Text(String content) : this() { this.content = content; }

        /// <summary>
        /// Create a Text class with content and position provided.
        /// </summary>
        public Text(String content, float x, float y) : this() { this.content = content; pos.X = x; pos.Y = y; }
        /// <summary>
        /// Create a Text class with no content and position provided.
        /// </summary>
        public Text(float x, float y) : this() { pos.X = x; pos.Y = y; }

        /// <summary>
        /// Set the font for the current Text.
        /// </summary>
        /// <param name="fontName">The font file name to load.</param>
        public void setFont(String fontName)
        {
            useFont = true;
            font = Useful.game.Content.Load<SpriteFont>(fontName);
        }

        /*
        public static void drawAll()
        {
            drawAll(Useful.spriteBatch);
        }
        */

        /// <summary>
        /// Draw all Texts using a provided SpriteBatch.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        {
            foreach (Text text in texts) text.draw(batch);
        }

        /// <summary>
        /// Draw the current text class using a provided SpriteBatch.
        /// </summary>
        public void draw(SpriteBatch batch)
        {
            if (visable)
            {
                if (useFont) batch.DrawString(font, content, pos, color);
                else batch.DrawString(normal, content, pos, color);
            }
        }

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }

        /// <summary>
        /// Moves the text in a certain direction.
        /// </summary>
        public void translate(Vector2 pos)
        {
            this.pos += pos;
        }

        /// <summary>
        /// Returns the size of the font in pixels when drawn in a Vector2 format.
        /// </summary>
        public Vector2 getSize()
        {
            return font.MeasureString(content);
        }
    }


    public static class Useful
    {
        internal static Game game;
        internal static SpriteBatch spriteBatch;

        /// <summary>
        /// Gets the window width of the application. A shortcut for GraphicsDevice.Viewport.Width.
        /// </summary>
        public static int getWWidth()
        {
            return game.GraphicsDevice.Viewport.Width;
        }

        /// <summary>
        /// Gets the window height of the application. A shortcut for GraphicsDevice.Viewport.Width.
        /// </summary>
        public static int getWHeight()
        {
            return game.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// Creates a filled rectangle texture2d.
        /// </summary>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="color">Color of the rectangle</param>
        /// <returns>A Texture2d Rectangle using the specified parameters.</returns>
        public static Texture2D CreateRectangle(int width, int height, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(game.GraphicsDevice, width, height);
            // create the rectangle texture, ,but it will have no color! lets fix that
            Color[] data = new Color[width * height];//set the color to the amount of pixels in the textures
            for (int i = 0; i < data.Length; i++)//loop through all the colors setting them to whatever values we want
            {
                data[i] = color;
            }
            rectangleTexture.SetData(data);//set the color data on the texture
            return rectangleTexture;//return the texture
        }

        /// <summary>
        /// Create a hollow box texture2d.
        /// </summary>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        /// <param name="color">Color of the box</param>
        /// <param name="depth">The amount of pixels for the width of the lines of the box.</param>
        /// <returns>A Texture2d Rectangle using the specified parameters.</returns>
        public static Texture2D CreateBox(int width, int height, int depth, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(game.GraphicsDevice,width,height);
            Color[,] data = new Color[height,width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    data[i, j] = Color.Transparent;
            for (int amount = 0; amount < depth; amount++)
            {
                for (int count = 0; count < height; count++)
                {
                    data[count,amount] = color;
                    data[count, width - 1 - amount] = color;
                }
                for (int count = 0; count < width; count++)
                {
                    data[amount,count] = color;
                    data[height-1-amount,count] = color;
                }
            }

            rectangleTexture.SetData(d2Tod1(data));//set the color data on the texture
            return rectangleTexture;//return the texture
        }
        /*
        public static void drawAll()
        {
            drawAll(spriteBatch);
        }
        */

        /// <summary>
        /// Draws all for both the Text Class and Sprite Class.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        {
            Sprite.drawAll(batch);
            Text.drawAll(batch);
        }

        /// <summary>
        /// Required before doing any of the methods in the SureDroid package.
        /// </summary>
        /// <param name="g">Game Class (This)</param>
        public static void set(Game g)
        {
            game = g;
            //spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        /// <summary>
        /// Converts a two dimentional color array to an one dimentional color array.
        /// </summary>
        private static Color[] d2Tod1(Color[,] array)
        {
            int width = array.GetLength(1);
            int height = array.GetLength(0);
            Color[] newArray = new Color[height * width];

            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    newArray[i * width + j] = array[i,j];
                }

            return newArray;
        }

        public static List<T> shuffle<T>(T Value, List<T> CList)
        {
            // Local Vars
            int I, R;
            bool Flag;
            Random Rand = new Random();
            // Local List of T type
            var CardList = new List<T>();
            // Build Local List as big as passed in list and fill it with default value
            for (I = 0; I < CList.Count; I++)
                CardList.Add(Value);
            // Shuffle the list of cards
            for (I = 0; I < CList.Count; I++)
            {
                Flag = false;
                // Loop until an empty spot is found
                do
                {
                    R = Rand.Next(0, CList.Count);
                    if (CardList[R].Equals(Value))
                    {
                        Flag = true;
                        CardList[R] = CList[I];
                    }
                } while (!Flag);
            }

            // Return the shuffled list
            return CardList;
        }
    }

    /* In Development (NonFunctional)
     * 
    public class KeyboardManager : GameComponent
    {
        KeyboardState kb, okb;
        List<Keys> touched, pressed;

        KeyboardManager() : base(Useful.game)
        {
            Useful.game.Components.Add(this);
        }

        public void addtouch(Keys key) { touched.Add(key); }
        public void addpressed(Keys key) { pressed.Add(key); }

        public override void Initialize()
        {
            okb = Keyboard.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Keys key in touched)
            base.Update(gameTime);
        }
    }


    
    public class Bar
    {
        Texture2D texture;
        public Bar(int max, int length)
        {

        }
    }
    */
}