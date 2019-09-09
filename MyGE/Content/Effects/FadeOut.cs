using Microsoft.Xna.Framework;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Effects
{
    public class FadeOut : ShaderEffect
    {
        public FadeOut() : base("FadeOut")
        {
            timer = new Timer(4, EndTimer);
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
