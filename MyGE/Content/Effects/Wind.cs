using MyGE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Effects
{
    public class Wind : ShaderEffect
    {
        public Wind(int pScreenW, int pScreenH) : base("Wind")
        {
            timer = new Timer(MathHelper.Pi * 2, EndTimer);
            Vector2 center = new Vector2(pScreenW / 2, pScreenH / 2);
            Shader.Parameters["viewMatrix"].SetValue(Matrix.CreateLookAt(new Vector3(center, 0), new Vector3(center, 1), new Vector3(0, -1, 0)));
            Shader.Parameters["projectionMatrix"].SetValue(Matrix.CreateOrthographic(center.X * 2, center.Y *2, 0, 1));

        }

        public void EndTimer(object sender, EventArgs e)
        {
            //Console.WriteLine("FadeOut timer Ended");
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            Shader.Parameters["time"].SetValue((float)timer.CurrentTime);
        }
    }
}
