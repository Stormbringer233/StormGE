using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class Frame : Widget
        /**
         * Define a frame that is a simple container component. A frame is like a window without topbar and
         * with any possibility to move.
         * A Frame can have a backround or not
         * A Frame is a base object that will contains other widgets to creates some complex ones like :
         *  - ascensors
         *  - radio buttons
         *  - ...
         * 
         * M. Le Thiec
         * Creation Date : 30/10/2018
         * 
         * */
    {

        public Frame(Widget pMaster, string pName, Vector2 pPosition, Rectangle pDimensions = new Rectangle())
        {
            Master = pMaster;
            Position = pPosition;
            if (pDimensions != new Rectangle())
                Quad = pDimensions;
            SetMovable(false);
        }

        public override void Add(Widget pElement)
        {
            base.Add(pElement);
            //int left = (int)Position.X - Quad.Width / 2;
            //int top = (int)Position.Y - Quad.Height / 2;
            //pElement.Position = new Vector2(left + pElement.Position.X, top + pElement.Position.Y);
            //WriteLine("new position for " + pElement.Name + " : " + pElement.Position);
            int left = 0;
            int top = 0;
            if (Master != null)
            {
                left = (int)Master.Position.X - Master.Quad.Width / 2;
                top = (int)Master.Position.Y - Master.Quad.Height / 2;
                pElement.Position = new Vector2(Position.X + left, Position.Y + top);
            }
            else
            {
                left = (int)Position.X - Quad.Width / 2;
                top = (int)Position.Y - Quad.Height / 2;
                pElement.Position = new Vector2(left + pElement.Position.X, top + pElement.Position.Y);
                //pElement.Position = Position;
            }

        }

        public override void UpdateStates()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
