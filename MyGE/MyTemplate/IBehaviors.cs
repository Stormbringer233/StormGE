using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public interface IBehaviors
        // interface for creating strategy pattern for player abstract class
        // The concrete class must implement this Interface
    {
        bool InMove { get; }

        //void Move(string pDirection);
        void Move(Point DPad);
        void Move(float dX, float dY);
        void SetAction();
        void Update(GameTime gameTime);
    }
}
