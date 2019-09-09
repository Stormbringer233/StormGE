using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class ProgressBar
    {
        public enum Ways { LEFT2RIGHT, RIGHT2LEFT};
        public Ways Way;
        public Vector2 Position;
        public int BarHeight;
        private Timer TweenTime;
        private Texture2D Texture;
        private int Lenght;
        private Vector2 Scale;
        private Color Foreground;
        private Color Background;
        private float Ratio;
        private float InitialScale;
        private float distance;
        private bool InUpdate;
        private int Life;
        private int InitialLife;
        private readonly float MAX_TIME = 1f;

        public ProgressBar(Texture2D pTexture, Vector2 pPosition, int pLenght, int pLife, Color pForeground, Color pBackground = new Color())
        {
            Texture = pTexture;
            Position = pPosition;
            Lenght = pLenght;
            BarHeight = Texture.Height;
            Life = pLife; // define the ratio between life and progress bar lenght
            // So Life represent 100% of the lenght
            InitialLife = pLife;
            Foreground = pForeground;
            Background = pBackground;
            InUpdate = false;
            distance = 0;
            Ratio = 1f;
            InitialScale = 1f;
            Scale = new Vector2(Lenght * Ratio, 1);
            Way = Ways.LEFT2RIGHT;
        }

        private void SetScale(float pAmong)
            // Consider to lost 100% of bar demand 5 second so :
            // new timer is : pAmong * 5
        {
            //Console.WriteLine("received among : " + pAmong);
            InitialScale = Ratio;
            Ratio *= pAmong;
            distance = Ratio - InitialScale;
            TweenTime = new Timer(Math.Abs(MAX_TIME * distance), OnTimerEnded);
            //Scale = new Vector2(Lenght * Ratio, 1);
            InUpdate = true;
            //Console.WriteLine(InitialScale + ", " + Ratio + " , " + distance);
        }

        public void HitPoint(int pPoints)
        {
            float initLife = Life;
            Life -= pPoints;
            if (Life < 0)
                Life = 0;
            float among = (Life / initLife);
            SetScale(among);
        }

        public void Reset()
        {
            distance = 1 - Ratio;
            Ratio = 1f;
            Life = InitialLife;
            InUpdate = true;
        }

        private void OnTimerEnded(object sender , EventArgs e)
        {
            InUpdate = false;
            Scale = new Vector2(Lenght * Ratio, 1);
            InitialScale = Ratio;
            //Console.WriteLine("Length of bar is : " + Lenght * Ratio);
        }

        public void Update(GameTime gameTime)
        {
            if (InUpdate)
            {
                Scale = new Vector2(Lenght * (float)EaseFunc.Linear(TweenTime.CurrentTime, InitialScale, distance, TweenTime.Duration), 1);
                //Console.WriteLine("Scale = " + Scale + "\t| CurrentTime = " + TweenTime.CurrentTime);
                
                TweenTime.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Draw(Texture, Position, null, Background, 0, Vector2.Zero, new Vector2(Lenght, 1), SpriteEffects.None, 0);
            sb.Draw(Texture, Position, null, Foreground, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
