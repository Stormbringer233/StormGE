using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    /// <summary>
    /// Draw simple primitives
    /// 
    /// M. Le Thiec
    /// date : jan 2019
    /// ver : 0.1
    /// 
    /// fev 2019
    /// Add offset for drawing rectangles
    /// </summary>
    public static class Primitive
    {
        public enum Types { LINE, FILL}
        public static Types type;
        public static Texture2D primitiveText;
        public static int lineWidth {get; set;}
        public static SpriteBatch sb;

        static Point Offset;

        public static void Initialize(SpriteBatch pSb, GraphicsDevice pGraphicsDevice)
        {
            sb = pSb;
            primitiveText = new Texture2D(pGraphicsDevice, 1, 1);
            primitiveText.SetData(new[] { Color.White });
            lineWidth = 1;
            Offset = Point.Zero;
        }

        public static void Dispose()
        {
            primitiveText.Dispose();
        }

        private static double Hypothenuse(int x0, int y0, int x1, int y1)
        {
            float x2 = (x1 - x0) * (x1 - x0);
            float y2 = (y1 - y0) * (y1 - y0);
            return Math.Sqrt((x2 + y2));
        }

        public static void DrawLine(int x0, int y0, int x1, int y1, Color pColor)
        {
            Vector2 Lenght = new Vector2(x1, y1) - new Vector2(x0, y0);
            float theta = (float)Math.Atan2(Lenght.Y, Lenght.X);
            sb.Draw(primitiveText, new Rectangle(x0, y0, (int)Lenght.Length(), lineWidth), null, pColor, theta, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Types pType, int x, int y, int width, int height, Color pColor)
        {
            if (pType == Types.FILL)
            {
                sb.Draw(primitiveText, new Rectangle(x + Offset.X, y + Offset.Y, width, height), pColor);
            }
            else
            {
                DrawLine(x + Offset.X, y + Offset.Y, x + width + Offset.X, y + Offset.Y, pColor);
                DrawLine(x + width + Offset.X, y + Offset.Y, x + width + Offset.X, y + height + Offset.Y, pColor);
                DrawLine(x + width + Offset.X, y + height + Offset.Y, x + Offset.X, y + height + Offset.Y, pColor);
                DrawLine(x + Offset.X, y + height + Offset.Y, x + Offset.X, y + Offset.Y, pColor);
            }
        }

        public static void DrawRectangle(Types pType, Rectangle pBound, Color pColor)
        {
            DrawRectangle(pType, pBound.X, pBound.Y, pBound.Width, pBound.Height, pColor);
        }

        public static void DrawRectangle(Types pType, Rectangle pBound, Point pOffset, Color pColor)
        {
            Offset = pOffset;
            DrawRectangle(pType, pBound.X, pBound.Y, pBound.Width, pBound.Height, pColor);
        }
    }
}
