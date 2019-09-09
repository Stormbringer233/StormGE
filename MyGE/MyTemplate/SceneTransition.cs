using MyGE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenes
{
    public abstract class SceneTransition
    {
        public delegate void TransitionEndedEventHandler(object sender, EventArgs e);
        //public event TransitionEndedEventHandler TransitionEnded; // allow a subscription by customer

        public bool OnTransition { get; set; }

        protected Timer SwitchSceneTimer;
        protected string NextScene;
        protected double SwitchTimer;
        protected TransitionEndedEventHandler EndedFunction;

        public SceneTransition(TransitionEndedEventHandler pFunction, double pSwitchTime)
        {
            SwitchTimer = pSwitchTime;
            SwitchSceneTimer = new Timer(SwitchTimer, OnTimerFinish);
            OnTransition = false;
            EndedFunction = pFunction;
        }

        protected virtual void OnTimerFinish(object sender, EventArgs e)
        {
            Console.WriteLine("\t-> SwitchSceneTimer Ended");
            OnTransition = false;
            EndedFunction?.Invoke(this, e);
        }

        public virtual void Update(GameTime gameTime)
        {
            SwitchSceneTimer.Update(gameTime);
        }

        public abstract void Draw(SpriteBatch sb, GameTime gameTime);
    }
}
