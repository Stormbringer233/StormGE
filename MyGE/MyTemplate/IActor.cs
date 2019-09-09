using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public interface IActor
    {
        Vector2 Position { get; set; }
        Vector2 Origin { get; set; } // define origin of rotate point
        void Update(GameTime gameTime);
        //void Draw(GameTime gameTime);
        void Draw(SpriteBatch spritebatch, GameTime gametime);
    }
}
