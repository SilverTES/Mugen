using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mugen.GUI;
using Mugen.Physics;


namespace Mugen.GFX
{
    public struct ColorI
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public override string ToString()
        {
            return "{ R=" + R + ", G=" + G + ", B=" + B + ", A=" + A + " }";
        }
    }
    public struct ColorF
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public override string ToString()
        {
            return "{ R=" + R + ", G=" + G + ", B=" + B + ", A=" + A + " }";
        }

    }

    public static class GFX
    {

        private static readonly Dictionary<string, List<Vector2>> circleCache = new Dictionary<string, List<Vector2>>();

        //static public SpriteFont? _defaultFont = null;

        static public Texture2D? _whitePixel = null;
        static public Rectangle _rect1x1 = new Rectangle(0, 0, 1, 1);
        static public Texture2D? _mouseCursor = null;
        static public Texture2D? _defaultSkinGui = null;
        static public Texture2D? _gamePadSNES = null;

        public static void Init(GraphicsDevice graphicsDevice) //, SpriteFont defaultFont)
        {
            //_defaultFont = defaultFont;

            _whitePixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whitePixel.SetData<Color>(new Color[] { Color.White });

            // Base64 mouseCursor PNG
            byte[] data = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAB3RJTUUH4wIBDzkR8RAd/AAAABd0RVh0U29mdHdhcmUAR0xEUE5HIHZlciAzLjRxhaThAAAACHRwTkdHTEQzAAAAAEqAKR8AAAAEZ0FNQQAAsY8L/GEFAAAABmJLR0QA/wD/AP+gvaeTAAAAZklEQVR4nKXTUQqAQAgEUE/rMTyEF64MVjKlmknwa+XhKoqqbkcImxLAH+QEzIxGEmCRBNydQhKIYJACMEjrAEUKsGaBIA1AkQZct/IFGQEEGYF4mBAIuCOPX1gtToVvx1aKmFvYAbW61k/bL6TmAAAAAElFTkSuQmCC");
            MemoryStream ms = new MemoryStream(data);
            _mouseCursor = Texture2D.FromStream(graphicsDevice, ms);

            // Base64 defaultSkin PNG
            data = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAAB3RJTUUH4wIIDDU4YFk7TwAAABd0RVh0U29mdHdhcmUAR0xEUE5HIHZlciAzLjRxhaThAAAACHRwTkdHTEQzAAAAAEqAKR8AAAAEZ0FNQQAAsY8L/GEFAAAABmJLR0QARABEAERSEuVjAAADfklEQVR4nO1YzWsTQRR/iYJpqtKmUSFKBdPWIrapFLEUFdEcVIiCBz8OheKleCh49KYHDx4Ez70p/iveW3vwgxrxi4BIUlBs60HW/mb3TWYn2Wyym5244oNfdufj/eb9ZubtZpaKxSLdfL1mXVl6ITCz+FggNZDtCOzHPOAEN5BZemPR4tM6SnctGhi3gftWUPspHOAEtwheDTg3fV6gUwG6HzjBLYPnQKavW3T7iUWjF2xwvRe4H3zgy/WOCMJsYUBeBZ7JoODZByfuZfBElmslENCNB3ZQrYCZR19l5omyUoQUwMHzTAYFi9AFiIF5FgEEj7LfFuI+7LddFlwsgIPm69BowYXs2JQLfu0qF65iEAzMV4C3BTBWdENt09vZX+FMkmn7Xg3u+6PWUJVcef6IauVVWZFIJFywLMsFv3Y2cIJbGPh/rhPlchjADfio8GsHB7icmM2twLePRPsOh+cBB7gcaxDQ6Yzr7Z5WqdgDdzrjejs4wOUlIG7WIKBbOeBpYXPAT0DczFwO1AcIlwN+AuJm/3Og12YuB/AGxUsobA6AA1xeAiIz7Q0a2LQ3enJq7h5l8pOyols5AE5wCwN//6D9Bg2bA+AAlxOz+RzYkwnuu3eosa5nBxr1sBLiQCNXIH32AO0/PkPDs5dDARzg0q22ME6ZU5eItvsIcE5gS7QC73nHDxy1hdN14tgf6o1/VuEtEeSzina4F59Vni2vWHfuP7RKc/NywLAAFzjB3S1OLxAGOpgfEWAhhdkzAqn+3W2B+3PgKl/kAjAoD4ggWASDg/OC2he+qGO+bq6qpwD88MDq7J0rXZP3flD7qlxRBy8F6Es/MXFSBNUJ4KNvRRMCxHvgU7lMX96/E4/VTHrQfr5u/hZI0S4BLnvWK77gAqcRgwrey3JGi1eti6VbArjnsle9LDv+nDvGVuDty2UhZu3Vqkvc1uYGpfrSDaK5HsC9aszBnFGb/CuhLjkH5nfV+zbjMiYgrtZUQDuzz1d9RUxb6BVoliMmraWATnKhV/Zv5kCz/d3OtRfmu4X+dpMChvN5WdnqWd9OLqhcUdtO/BwtnBCF0WOTrkZdhG7Ntg5zMGfU5tpC+BtQ/fCZql8rrhxQc0INXK2HD3z1vyNGBGDJDx0ZERW1jXW7pW+HwBb9EuCyZ73iCy5j2yj2B5rYHyljf6iP+2eVP5Ce/XLygBXoAAAAAElFTkSuQmCC");
            ms = new MemoryStream(data);
            _defaultSkinGui = Texture2D.FromStream(graphicsDevice, ms);

            // Base64 gamePadSNES PNG
            data = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAQAAAABkCAYAAABpYO6eAAAAB3RJTUUH4QUJFBYkn5D41wAAABd0RVh0U29mdHdhcmUAR0xEUE5HIHZlciAzLjRxhaThAAAACHRwTkdHTEQzAAAAAEqAKR8AAAAEZ0FNQQAAsY8L / GEFAAAABmJLR0QA / wD / AP + gvaeTAAAKQ0lEQVR4nO1dObIeNRB + CycwCRzAkEGOAw4ARWgHUHAKOAQRV6DKiXMiYnwAQp4PQII5Ac9DyaBHo1 / SaOlV6q76yn7zz0i9fiNptqvjOK604eln3x4WIO2nUXz66Oej9q81SOeB5XwRVyAXQGl9evSW1mEUKxW / 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");
            ms = new MemoryStream(data);
            _gamePadSNES = Texture2D.FromStream(graphicsDevice, ms);

        }

        #region Draw Texture Methods
        public static void Draw(SpriteBatch batch, Texture2D texture, Color color, float rotation, Vector2 position, Vector2 origin, Vector2 scale, SpriteEffects spriteEffect = SpriteEffects.None, float layerDepth = 0f)
        {
            batch.Draw(texture, position, texture.Bounds, color, rotation, origin, scale, spriteEffect, layerDepth);
        }
        public static void Draw(SpriteBatch batch, Texture2D texture, Color color, float rotation, Vector2 position, Position origin, Vector2 scale, SpriteEffects spriteEffect = SpriteEffects.None, float layerDepth = 0f)
        {
            Vector2 pivot = new();
            int w = texture.Width;
            int h = texture.Height;

            switch (origin)
            {
                case Position.M:
                    pivot.X = w / 2; pivot.Y = h / 2;
                    break;
                case Position.NW:
                    pivot.X = w; pivot.Y = 0;
                    break;
                case Position.NE:
                    pivot.X = 0; pivot.Y = 0;
                    break;
                case Position.SW:
                    pivot.X = w; pivot.Y = h;
                    break;
                case Position.SE:
                    pivot.X = 0; pivot.Y = h;
                    break;
                case Position.N:
                    pivot.Y = 0;
                    break;
                case Position.S:
                    pivot.Y = h;
                    break;
                case Position.W:
                    pivot.X = w;
                    break;
                case Position.E:
                    pivot.X = 0;
                    break;
                case Position.NM:
                    pivot.X = w / 2; pivot.Y = 0;
                    break;
                case Position.SM:
                    pivot.X = w / 2; pivot.Y = h;
                    break;
                case Position.WM:
                    pivot.X = 0; pivot.Y = h / 2;
                    break;
                case Position.EM:
                    pivot.X = w; pivot.Y = h / 2;
                    break;
                default:
                    break;
            }

            Draw(batch, texture, color, rotation, position, pivot, scale, spriteEffect, layerDepth);
        }
        #endregion

        #region Draw Text Methods
        public static void BorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 pos, Color colorFG, Color colorBG)
        {
            batch.DrawString(font, text, pos + new Vector2(-1, 0), colorBG);
            batch.DrawString(font, text, pos + new Vector2(1, 0), colorBG);
            batch.DrawString(font, text, pos + new Vector2(0, -1), colorBG);
            batch.DrawString(font, text, pos + new Vector2(0, 1), colorBG);

            batch.DrawString(font, text, pos, colorFG);
        }
        // Align 
        public static void LeftTopString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x, y), color);
        }
        public static void LeftTopString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, position, color);
        }
        public static void LeftMiddleString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x, y - font.MeasureString(text).Y / 2), color);
        }
        public static void LeftMiddleString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X, position.Y - font.MeasureString(text).Y / 2), color);
        }
        public static void LeftBottomString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x, y - font.MeasureString(text).Y), color);
        }
        public static void LeftBottomString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X, position.Y - font.MeasureString(text).Y), color);
        }
        public static void RightTopString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X, y), color);
        }
        public static void RightTopString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y), color);
        }
        public static void RightMiddleString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X, y - font.MeasureString(text).Y / 2), color);
        }
        public static void RightMiddleString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y - font.MeasureString(text).Y / 2), color);
        }
        public static void RightBottomString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X, y - font.MeasureString(text).Y), color);
        }
        public static void RightBottomString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y - font.MeasureString(text).Y), color);
        }
        public static void LeftTopBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x, y), colorFG, colorBG);
        }
        public static void LeftTopBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, position, colorFG, colorBG);
        }
        public static void LeftMiddleBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x, y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }
        public static void LeftMiddleBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X, position.Y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }
        public static void LeftBottomBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x, y - font.MeasureString(text).Y), colorFG, colorBG);
        }
        public static void LeftBottomBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X, position.Y - font.MeasureString(text).Y), colorFG, colorBG);
        }
        public static void RightTopBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X, y), colorFG, colorBG);
        }
        public static void RightTopBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y), colorFG, colorBG);
        }
        public static void RightMiddleBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X, y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }
        public static void RightMiddleBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }
        public static void RightBottomBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X, y - font.MeasureString(text).Y), colorFG, colorBG);
        }
        public static void RightBottomBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X, position.Y - font.MeasureString(text).Y), colorFG, colorBG);
        }

        // Align Center X, Y,  XY
        public static void TopCenterString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X / 2, y), color);
        }
        public static void TopCenterString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y), color);
        }
        public static void BottomCenterString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X / 2, y - font.MeasureString(text).Y), color);
        }
        public static void BottomCenterString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y - font.MeasureString(text).Y), color);
        }
        public static void CenterStringXY(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color color)
        {
            batch.DrawString(font, text, new Vector2(x - font.MeasureString(text).X / 2, y - font.MeasureString(text).Y / 2), color);
        }
        public static void CenterStringXY(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color)
        {
            batch.DrawString(font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y - font.MeasureString(text).Y / 2), color);
        }
        public static void TopCenterBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X / 2, y), colorFG, colorBG);
        }
        public static void TopCenterBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y), colorFG, colorBG);
        }
        public static void BottomCenterBorderedString(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X / 2, y - font.MeasureString(text).Y), colorFG, colorBG);
        }
        public static void BottomCenterBorderedString(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y - font.MeasureString(text).Y), colorFG, colorBG);
        }
        public static void CenterBorderedStringXY(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(x - font.MeasureString(text).X / 2, y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }
        public static void CenterBorderedStringXY(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG, Color colorBG)
        {
            BorderedString(batch, font, text, new Vector2(position.X - font.MeasureString(text).X / 2, position.Y - font.MeasureString(text).Y / 2), colorFG, colorBG);
        }

        public static void String(SpriteBatch batch, SpriteFont font, string text, float x, float y, Color colorFG,
                                  Style.HorizontalAlign horizontalAlign = Style.HorizontalAlign.Center,
                                  Style.VerticalAlign verticalAlign = Style.VerticalAlign.Middle,
                                 bool bordered = false, Color colorBG = default)
        {
            if (!bordered)
            {
                if (horizontalAlign == Style.HorizontalAlign.Left)
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) LeftMiddleString(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Top) LeftTopString(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) LeftBottomString(batch, font, text, x, y, colorFG);
                }
                else if (horizontalAlign == Style.HorizontalAlign.Right)
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) RightMiddleString(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Top) RightTopString(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) RightBottomString(batch, font, text, x, y, colorFG);
                }
                else
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) CenterStringXY(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Top) TopCenterString(batch, font, text, x, y, colorFG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) BottomCenterString(batch, font, text, x, y, colorFG);
                }
            }
            else
            {
                if (horizontalAlign == Style.HorizontalAlign.Left)
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) LeftMiddleBorderedString(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Top) LeftTopBorderedString(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) LeftBottomBorderedString(batch, font, text, x, y, colorFG, colorBG);
                }
                else if (horizontalAlign == Style.HorizontalAlign.Right)
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) RightMiddleBorderedString(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Top) RightTopBorderedString(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) RightBottomBorderedString(batch, font, text, x, y, colorFG, colorBG);
                }
                else
                {
                    if (verticalAlign == Style.VerticalAlign.Middle) CenterBorderedStringXY(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Top) TopCenterBorderedString(batch, font, text, x, y, colorFG, colorBG);
                    if (verticalAlign == Style.VerticalAlign.Bottom) BottomCenterBorderedString(batch, font, text, x, y, colorFG, colorBG);
                }
            }

        }
        public static void String(SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color colorFG,
                                  Style.HorizontalAlign horizontalAlign = Style.HorizontalAlign.Center,
                                  Style.VerticalAlign verticalAlign = Style.VerticalAlign.Middle,
                                 bool bordered = false, Color colorBG = default)
        {
            String(batch, font, text, position.X, position.Y, colorFG, horizontalAlign, verticalAlign, bordered, colorBG);
        }

        #endregion

        #region Private Methods
        private static void Points(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
            {
                LineIn(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
        }
        private static List<Vector2> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = radius + "x" + sides;
            if (circleCache.ContainsKey(circleKey))
            {
                return circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

            // Cache this circle so that it can be quickly drawn next time
            circleCache.Add(circleKey, vectors);

            return vectors;
        }
        private static List<Vector2> CreateArc(float radius, int sides, float startingAngle, float radians)
        {
            List<Vector2> points = new List<Vector2>();
            points.AddRange(CreateCircle(radius, sides));
            points.RemoveAt(points.Count - 1); // remove the last point because it's a duplicate of the first

            // The circle starts at (radius, 0)
            double curAngle = 0.0;
            double anglePerSide = MathHelper.TwoPi / sides;

            // "Rotate" to the starting point
            while ((curAngle + (anglePerSide / 2.0)) < startingAngle)
            {
                curAngle += anglePerSide;

                // move the first point to the end
                points.Add(points[0]);
                points.RemoveAt(0);
            }

            // Add the first point, just in case we make a full circle
            points.Add(points[0]);

            // Now remove the points at the end of the circle to create the arc
            int sidesInArc = (int)((radians / anglePerSide) + 0.5);
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1);

            return points;
        }
        #endregion

        public static void Line(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1)
        {
            Line(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }
        public static void Line(SpriteBatch batch, Line line, Color color, float thickness = 1)
        {
            Line(batch, line.A, line.B, color, thickness);
        }
        public static void Line(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            Line(spriteBatch, point1, distance, angle, color, thickness);
        }
        public static void Line(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(_whitePixel,
                             point,
                             null,
                             color,
                             angle,
                             new Vector2(0f, 0.5f),
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);
        }

        public static void LineIn(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1)
        {
            LineIn(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }
        public static void LineIn(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            LineIn(spriteBatch, point1, distance, angle, color, thickness);
        }
        public static void LineIn(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(_whitePixel,
                             point,
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);
        }
        public static void Triangle(GraphicsDevice device, Triangle triangle, Color color)
        {
            BasicEffect _effect = new BasicEffect(device);
            _effect.Texture = _whitePixel;
            _effect.TextureEnabled = true;
            //_effect.VertexColorEnabled = true;

            VertexPositionTexture[] _vertices = new VertexPositionTexture[3];

            _vertices[0].Position = triangle.A;
            _vertices[1].Position = triangle.B;
            _vertices[2].Position = triangle.C;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives<VertexPositionTexture>
                (
                    PrimitiveType.TriangleStrip, // same result with TriangleList
                    _vertices,
                    0,
                    _vertices.Length,
                    new int[] { 0, 1, 2 },
                    0,
                    1
                );
            }

        }
        public static void Polygon(SpriteBatch spriteBatch, Vector2[] vertex, Color color, float thickness = 1, Vector2 offset = default)
        {
            if (vertex.Length > 0)
            {
                for (int i = 0; i < vertex.Length - 1; i++)
                {
                    Line(spriteBatch, vertex[i] + offset, vertex[i + 1] + offset, color, thickness);
                }

                Line(spriteBatch, vertex[vertex.Length - 1] + offset, vertex[0] + offset, color, thickness);
            }
        }
        public static void PolyLine(SpriteBatch spriteBatch, Vector2[] vertex, Color color, float thickness = 1, Vector2 offset = default)
        {
            if (vertex.Length > 0)
            {
                for (int i = 0; i < vertex.Length - 1; i++)
                {
                    Line(spriteBatch, vertex[i] + offset, vertex[i + 1] + offset, color, thickness);
                }
            }
        }

        public static void Ellipse(SpriteBatch batch, float x, float y, float rX, float rY, int side, Color color, float size = 1)
        {
            double angle = 0;
            float prevX = (float)Math.Cos(angle) * rX;
            float prevY = (float)Math.Sin(angle) * rY;

            float curX = 0;
            float curY = 0;

            double twoPI = Math.PI * 2;
            double step = twoPI / side;

            for (int i = 1; i < side + 1; ++i)
            {
                angle = step * i;

                curX = (float)Math.Cos(angle) * rX;
                curY = (float)Math.Sin(angle) * rY;

                Line(batch, prevX + x, prevY + y, curX + x, curY + y, color, size);

                prevX = curX;
                prevY = curY;
            }
        }
        public static void Ellipse(SpriteBatch batch, Vector2 pos, Vector2 radius, int side, Color color, float size = 1)
        {
            Ellipse(batch, pos.X, pos.Y, radius.X, radius.Y, side, color, size);
        }

        public static void FillRectangle(SpriteBatch batch, Rectangle rect, Color color)
        {
            batch.Draw(_whitePixel, rect, color);
        }
        public static void FillRectangle(SpriteBatch batch, RectangleF rect, Color color)
        {
            batch.Draw(_whitePixel, new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height), color);
        }
        public static void FillRectangle(SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float angle)
        {
            spriteBatch.Draw(_whitePixel,
                             location,
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             size,
                             SpriteEffects.None,
                             0);
        }
        public static void FillRectangleCentered(SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float angle)
        {
            spriteBatch.Draw(_whitePixel,
                             location,
                             null,
                             color,
                             angle,
                             new Vector2(.5f, .5f),
                             size,
                             SpriteEffects.None,
                             0);
        }
        public static void FillRectangle(SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color)
        {
            FillRectangle(spriteBatch, location, size, color, 0.0f);
        }
        public static void FillRectangle(SpriteBatch spriteBatch, float x, float y, float w, float h, Color color)
        {
            FillRectangle(spriteBatch, new Vector2(x, y), new Vector2(w, h), color, 0.0f);
        }

        public static void FillSquare(SpriteBatch spriteBatch, Vector2 location, float size, Color color)
        {
            FillRectangleCentered(spriteBatch, location, new Vector2(size), color, 0.0f);
        }

        public static void Point(SpriteBatch spriteBatch, Vector2 location, float size, Color color)
        {
            Circle(spriteBatch, location, size, 4 + (int)size, color, size);
        }
        public static void Point(SpriteBatch spriteBatch, float x, float y, float size, Color color)
        {
            Circle(spriteBatch, new Vector2(x, y), size, 4 + (int)size, color, size);
        }

        public static Vector2[] RectangleEx(SpriteBatch spriteBatch, Vector2 origin, RectangleF rect, Vector2 offset, Color color, float rotation = 0f, bool isRender = true, float thickness = 1f, bool isShowCenter = false)
        {
            Vector2[] vertexs = new Vector2[8];

            vertexs[RectangleF.VertexTopLeft] = rect.TopLeft + offset;
            vertexs[RectangleF.VertexTopCenter] = rect.TopCenter + offset;
            vertexs[RectangleF.VertexTopRight] = rect.TopRight + offset;
            vertexs[RectangleF.VertexRightMiddle] = rect.RightMiddle + offset;
            vertexs[RectangleF.VertexBottomRight] = rect.BottomRight + offset;
            vertexs[RectangleF.VertexBottomCenter] = rect.BottomCenter + offset;
            vertexs[RectangleF.VertexBottomLeft] = rect.BottomLeft + offset;
            vertexs[RectangleF.VertexLeftMiddle] = rect.LeftMiddle + offset;

            Shape shape = new Shape(origin.X, origin.Y, vertexs, true);

            shape._angle = rotation;
            shape.Transform();
            //shape.Update();
            if (isRender)
                shape.Render(spriteBatch, color, thickness, isShowCenter);

            return shape._vertexsFinal;

        }
        public static void Rectangle(SpriteBatch spriteBatch, RectangleF rect, Color color, float thickness = 1f)
        {
            // TODO: Figure out the pattern for the offsets required and then handle it in the line instead of here
            float offset = thickness / 2;
            rect = RectangleF.Translate(rect, new Vector2(.5f, .5f));

            Line(spriteBatch, new Vector2(rect.X - offset, rect.Y), new Vector2(rect.Right + offset, rect.Y), color, thickness); // top
            Line(spriteBatch, new Vector2(rect.X - offset, rect.Bottom), new Vector2(rect.Right + offset, rect.Bottom), color, thickness); // bottom
            Line(spriteBatch, new Vector2(rect.X, rect.Y - offset), new Vector2(rect.X, rect.Bottom + offset), color, thickness); // left
            Line(spriteBatch, new Vector2(rect.Right, rect.Y - offset), new Vector2(rect.Right, rect.Bottom + offset), color, thickness); // right
        }
        public static void RectangleI(SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness = 1f)
        {
            Rectangle(spriteBatch, rect, color, thickness);
        }
        public static void Rectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float thickness = 1f)
        {
            Rectangle(spriteBatch, new RectangleF(x, y, width, height), color, thickness);
        }
        public static void Rectangle(SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float thickness = 1f)
        {
            Rectangle(spriteBatch, new RectangleF((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, thickness);
        }
        public static void RectangleCentered(SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float thickness = 1f)
        {
            Rectangle(spriteBatch, new RectangleF((int)location.X - size.X / 2, (int)location.Y - size.Y / 2, (int)size.X, (int)size.Y), color, thickness);
        }
        public static void Circle(SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f)
        {
            Points(spriteBatch, center, CreateCircle(radius, sides), color, thickness);
        }
        public static void Circle(SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f)
        {
            Points(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, thickness);
        }
        public static void Arc(SpriteBatch spriteBatch, Vector2 center, float radius, int sides, float startingAngle, float radians, Color color, float thickness)
        {
            List<Vector2> arc = CreateArc(radius, sides, startingAngle, radians);
            //List<Vector2> arc = CreateArc2(radius, sides, startingAngle, degrees);
            Points(spriteBatch, center, arc, color, thickness);
        }
        public static void RoundedRectangle(SpriteBatch spriteBatch, RectangleF rect, float radius, int sides, Color color, float thickness = 1f)
        {
            Arc(spriteBatch, rect.TopLeft + Vector2.One * radius, radius, sides, Geo.RAD_180, Geo.RAD_90, color, thickness);
            Line(spriteBatch, rect.TopLeft + Vector2.UnitX * radius, rect.TopRight - Vector2.UnitX * radius, color, thickness);
            Arc(spriteBatch, rect.TopRight - Vector2.UnitX * radius + Vector2.UnitY * radius, radius, sides, Geo.RAD_270, Geo.RAD_90, color, thickness);
            Line(spriteBatch, rect.TopRight + Vector2.UnitY * radius, rect.BottomRight - Vector2.UnitY * radius, color, thickness);
            Arc(spriteBatch, rect.BottomLeft + Vector2.UnitX * radius - Vector2.UnitY * radius, radius, sides, Geo.RAD_90, Geo.RAD_90, color, thickness);
            Line(spriteBatch, rect.BottomLeft + Vector2.UnitX * radius, rect.BottomRight - Vector2.UnitX * radius, color, thickness);
            Arc(spriteBatch, rect.BottomRight - Vector2.One * radius, radius, sides, -Geo.RAD_90, Geo.RAD_90, color, thickness);
            Line(spriteBatch, rect.TopLeft + Vector2.UnitY * radius, rect.BottomLeft - Vector2.UnitY * radius, color, thickness);
        }
        public static void BevelledRectangle(SpriteBatch spriteBatch, RectangleF rect, Vector2 bevel, Color color, float thickness = 1f)
        {
            Line(spriteBatch, rect.TopLeft + Vector2.UnitY * bevel.Y, rect.TopLeft + Vector2.UnitX * bevel.X, color, thickness);
            Line(spriteBatch, rect.TopLeft + Vector2.UnitX * bevel.X, rect.TopRight - Vector2.UnitX * bevel.X, color, thickness);
            Line(spriteBatch, rect.TopRight - Vector2.UnitX * bevel.X, rect.TopRight + Vector2.UnitY * bevel.Y, color, thickness);
            Line(spriteBatch, rect.TopRight + Vector2.UnitY * bevel.Y, rect.BottomRight - Vector2.UnitY * bevel.Y, color, thickness);
            Line(spriteBatch, rect.BottomRight - Vector2.UnitY * bevel.Y, rect.BottomRight - Vector2.UnitX * bevel.X, color, thickness);
            Line(spriteBatch, rect.BottomLeft + Vector2.UnitX * bevel.X, rect.BottomRight - Vector2.UnitX * bevel.X, color, thickness);
            Line(spriteBatch, rect.BottomLeft + Vector2.UnitX * bevel.X, rect.BottomLeft - Vector2.UnitY * bevel.Y, color, thickness);
            Line(spriteBatch, rect.TopLeft + Vector2.UnitY * bevel.Y, rect.BottomLeft - Vector2.UnitY * bevel.Y, color, thickness);
        }

        public static void PutPixel(SpriteBatch spriteBatch, float x, float y, Color color)
        {
            PutPixel(spriteBatch, new Vector2(x, y), color);
        }
        public static void PutPixel(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(_whitePixel, position, color);
        }

        public static void Bar(SpriteBatch spriteBatch, float x, float y, float value, float height, Color color)
        {
            Line(spriteBatch, x, y, x + value, y, color, height);
        }
        public static void Bar(SpriteBatch spriteBatch, Vector2 position, float value, float height, Color color)
        {
            Line(spriteBatch, position.X, position.Y, position.X + value, position.Y, color, height);
        }
        public static void BarLines(SpriteBatch spriteBatch, Vector2 position, float value, float height, Color color, float thickness = 1f)
        {
            float h = height / 2;
            Line(spriteBatch, position.X, position.Y - h, position.X + value, position.Y - h, color, thickness);
            Line(spriteBatch, position.X, position.Y + h, position.X + value, position.Y + h, color, thickness);
            Line(spriteBatch, position.X + value, position.Y - h, position.X + value, position.Y + h, color, thickness);
            Line(spriteBatch, position.X, position.Y - h, position.X, position.Y + h, color, thickness);
        }
        public static void Triangle(SpriteBatch batch, float x1, float y1, float x2, float y2, float x3, float y3, Color color, float size = 1)
        {
            Vector2 p1 = new Vector2(x1, y1);
            Vector2 p2 = new Vector2(x2, y2);
            Vector2 p3 = new Vector2(x3, y3);

            Line(batch, p1, p2, color, size);
            Line(batch, p2, p3, color, size);
            Line(batch, p3, p1, color, size);
        }

        public static void Sight(SpriteBatch batch, float x, float y, int screenW, int screenH, Color color, float thickness = 1f)
        {
            Line(batch, x + .5f, 0, x + .5f, screenH, color, thickness);
            Line(batch, 0, y + .5f, screenW, y + .5f, color, thickness);
        }
        public static void Sight(SpriteBatch batch, Vector2 position, int screenW, int screenH, Color color, float thickness = 1f)
        {
            Sight(batch, position.X, position.Y, screenW, screenH, color, thickness);
        }
        public static void Grid(SpriteBatch batch, float x, float y, float gridW, float gridH, int cellW, int cellH, Color color, float thickness = 1f)
        {
            for (int i = 0; i < Math.Floor(gridW / cellW) + 1; i++)
            {
                Line(batch,
                    (float)Math.Floor(i * cellW + x) + .5f, (float)Math.Floor(y) + .5f,
                    (float)Math.Floor(i * cellW + x) + .5f, (float)Math.Floor(gridH + y) + .5f,
                    color, thickness);
            }

            for (int i = 0; i < Math.Floor(gridH / cellH) + 1; i++)
            {
                Line(batch,
                    (float)Math.Floor(x) + .5f, (float)Math.Floor(i * cellH + y) + .5f,
                    (float)Math.Floor(gridW + x) + .5f, (float)Math.Floor(i * cellH + y) + .5f,
                    color, thickness);
            }
        }
        public static void Grid(SpriteBatch batch, Vector2 position, float gridW, float gridH, int cellW, int cellH, Color color, float thickness = 1f)
        {
            Grid(batch, position.X, position.Y, gridW, gridH, cellW, cellH, color, thickness);
        }
        public static void Mosaic(SpriteBatch batch, Rectangle rectView, float x, float y, int repX, int repY, Texture2D bitmap, Color color)
        {
            float tileW = bitmap.Width;
            float tileH = bitmap.Height;

            for (int i = 0; i < repX; ++i)
            {
                if (x + i * tileW < rectView.X - tileW || x + i * tileW > rectView.X + rectView.Width)
                    continue;

                for (int j = 0; j < repY; ++j)
                {
                    if (y + j * tileH < rectView.Y - tileH || y + j * tileH > rectView.Y + rectView.Height)
                        continue;

                    batch.Draw
                    (
                        bitmap,
                        new Rectangle((int)Math.Floor(x + i * tileW), (int)Math.Floor(y + j * tileH), (int)tileW, (int)tileH),
                        new Rectangle(0, 0, (int)tileW, (int)tileH),
                        color
                    );
                }
            }
        }
        public static void Mosaic(SpriteBatch batch, Rectangle rectView, float x, float y, int repX, int repY, Texture2D bitmap, float tileX, float tileY, float tileW, float tileH, Color color)
        {

            for (int i = 0; i < repX; ++i)
            {
                if (x + i * tileW < rectView.X - tileW || x + i * tileW > rectView.X + rectView.Width)
                    continue;

                for (int j = 0; j < repY; ++j)
                {
                    if (y + j * tileH < rectView.Y - tileH || y + j * tileH > rectView.Y + rectView.Height)
                        continue;

                    batch.Draw
                    (
                        bitmap,
                        new Rectangle((int)Math.Floor(x + i * tileW), (int)Math.Floor(y + j * tileH), (int)tileW, (int)tileH),
                        new Rectangle(0, 0, (int)tileW, (int)tileH),
                        color
                    );
                }
            }
        }
        public static void Mosaic(SpriteBatch batch, Rectangle rectView, float x, float y, int repX, int repY, Texture2D bitmap, Rectangle rect, Color color)
        {

            for (int i = 0; i < repX; ++i)
            {
                if (x + i * rect.Width < rectView.X - rect.Width || x + i * rect.Width > rectView.X + rectView.Width)
                    continue;

                for (int j = 0; j < repY; ++j)
                {
                    if (y + j * rect.Height < rectView.Y - rect.Height || y + j * rect.Height > rectView.Y + rectView.Height)
                        continue;

                    batch.Draw
                    (
                        bitmap,
                        new Rectangle((int)Math.Floor(x + i * rect.Width), (int)Math.Floor(y + j * rect.Height), rect.Width, rect.Height),
                        rect,
                        color
                    );
                }
            }
        }
    }
}
