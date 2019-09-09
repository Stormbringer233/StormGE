using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GUI
{
    class Canvas : Widget
    {
        /// <summary>
        /// A Canvas is a container that can embed somme graphics element like image, graphics and so on
        /// </summary>
        public Canvas(Widget pMaster, string pName, Vector2 pPosition)
        {
            Master = pMaster;
            Name = pName;
            Position = pPosition;
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void UpdateStates()
        {
            throw new NotImplementedException();
        }
    }
}
