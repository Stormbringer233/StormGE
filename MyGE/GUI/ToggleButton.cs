using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTemplate;

namespace GUI
{
    public class ToggleButton : Button
    {
        /** 
         * Define the behavior of Toggle Button. It derived from Button
         * A ToggleButton is like a simple button but, it doesn't engage any function. A tooggleButton is
         * define to store a boolean flag.
         * 
         * M. Le Thiec
         * Creation date : 28/10/2018
         * 
         * */

        protected bool StoredFlag;

        public ToggleButton(Widget pMaster, string pName, Vector2 pPosition, ClickEvent pFunction) :
            base (pMaster, pName, pPosition, pFunction)
        {
            StoredFlag = false;
            UpdateQuad(); // reupdate Quad for ToggleButton specificities
        }

        public void Toggle()
            // Simply toggle the state of the flag
        {
            StoredFlag = (StoredFlag == false) ? true : false;
            Function?.Invoke();
        }

        private int GetFrame()
            // Calculate the right frame number according to state and flag
            // The frame arragment is 3 frames per behavior, so
            // off/normal -> off/mouseOver -> off/Freeze
            // on/normal -> on/MouseOver -> on/Freeze
            // All of quads on an single line in the sheet
        {
            int frame = 0; // equivalent of off/Normal
            if (State == States.MOUSEOVER && StoredFlag == false)
                frame = 1;
            else if (State == States.FREEZE && StoredFlag == false)
                frame = 2;
            else if (State == States.NORMAL && StoredFlag == true)
                frame = 3;
            else if (State == States.MOUSEOVER && StoredFlag == true)
                frame = 4;
            else if (State == States.FREEZE && StoredFlag == true)
                frame = 5;
            return frame;
        }

        protected override void UpdateQuad()
            // Toggle button must override these function because, it manage 4 states and 6 quads
            // according to toggle button has 2 images for mouseOver state and freeze state
            // So, the updating is made with states and flag state
        {
            //int frame = ((int)State <= Quads.Count - 1) ? (int)State : 0;
            int frame = GetFrame();
            Quad = new Rectangle(Quads[frame].X, Quads[frame].Y, Quads[frame].Width, Quads[frame].Height);
            UpdateBound();
        }

        public override void UpdateStates()
            // The toggle button has 4 states, but 6 images
            // - normal state
            // - mouseOver state
            // engage state
            // freeze state
        {
            if (State != States.FREEZE)
            {
                Point mousePos = MouseWrapper.Position;
                bool hasFocus = CollidePoint(mousePos);

                if (hasFocus && MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.NONE)
                {
                    // simply change quad
                    State = States.MOUSEOVER;
                }
                else if (hasFocus &&
                        (MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.ENGAGE ||
                        MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.DRAG) && State == States.MOUSEOVER)
                {
                    State = States.CLICKED;
                    Toggle();
                    //Console.WriteLine("button state is clicked");
                }
                else if (!hasFocus)
                {
                    State = States.NORMAL;
                }
                UpdateQuad();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsShown())
            {
                UpdateStates();
            }
        }

        
    }
}
