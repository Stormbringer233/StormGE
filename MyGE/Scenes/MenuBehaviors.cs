using MyTemplate;
using Microsoft.Xna.Framework;
using System;

namespace Scenes
{
    class MenuBehaviors : IBehaviors
    {
        public bool InMove { get; private set;}

        public void Move(float dX, float dY) { }

        public void Move(Point DPad)
        {
            if (DPad.Y == 1)
                Console.WriteLine("Move menu on DOWN");
            else if (DPad.Y == -1)
                Console.WriteLine("Move menu on TOP");
        }

        public void SetAction()
        {

        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
