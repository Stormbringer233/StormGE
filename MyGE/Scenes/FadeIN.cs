using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;

namespace Scenes
{
    class FadeInTransition : SceneTransition
    {
        Color fadeColor;
        float alpha;
        float step;

        public FadeInTransition(TransitionEndedEventHandler pFunction, double pSwitchTime = 0.5) : base(pFunction, pSwitchTime)
        {
            //SwitchSceneTimer.ProcessFinish += OnTimerFinish;
            fadeColor = Color.Black;
            alpha = 0;
            step =  1 / (float)(SwitchTimer / 0.016); // 0.016 = time between 2 updates
        }

        protected override void OnTimerFinish(object sender, EventArgs e)
        {
            alpha = 0;
            base.OnTimerFinish(sender, e);
        }

        public override void Update(GameTime gameTime)
        {
            if (OnTransition)
            {
                //Console.WriteLine("FadeIn update()");
                alpha += step;
                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (OnTransition)
                Primitive.DrawRectangle(Primitive.Types.FILL, 0, 0, MainGame.SCREEN_WIDTH, MainGame.SCREEN_HEIGHT, fadeColor * alpha);
        }
    }
}
