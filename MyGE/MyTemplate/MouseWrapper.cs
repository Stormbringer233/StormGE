using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MyTemplate
{
    /** MouseWrapper class provide common feature to manage mouse behavior like :
     *  - button management state :
     *      * detection of Pressed and Released actions
     *      * detection of clic state (none, simple_clic, double_clic and drag) modes
     *  - common screen position management :
     *      * simple position
     *      * delta position
     *  - TODO : World position computing (need matrix transformation)
     *  
     *  M. Le Thiec
     *  V 0.50
     *  */

    public class MouseButton
        // Manage all of mouse button and store states.
    {
        public delegate void SimpleClicEventHandler(object sender, EventArgs e);
        public event SimpleClicEventHandler SimpleClicEvent;

        public enum ClicStates {NONE, SIMPLE_CLIC, ENGAGE, DRAG, DOUBLE_CLIC };
        public ClicStates ClicState { get; set; }
        public ButtonState BTNState { get; set; }
        public ButtonState OldBtnState { get; set; }

        public Timer clicTime;
        

        double DoubleClicDelay;

        public MouseButton(double pDoubleClicDelay)
        {
            ClicState = ClicStates.NONE;
            BTNState = ButtonState.Released;
            OldBtnState = ButtonState.Released;
            DoubleClicDelay = pDoubleClicDelay;
            clicTime = new Timer();
            clicTime.Pause();
        }

        private void Reset()
        {
            ClicState = ClicStates.NONE;
            clicTime.Flush(); // reinitialize timer
            clicTime.Pause(); // banned at the timer count restarting
        }

        public void Update(GameTime gameTime)
        {
            clicTime.Update(gameTime);
            //Console.WriteLine("ClicState is " + ClicState + "\t|\tUpdate Button - State = " + BTNState + "\t|\tOldBtnState = " + OldBtnState +
            //    "\t|\tClicTime = " + clicTime.CurrentTime);
            if (BTNState == ButtonState.Pressed && OldBtnState == ButtonState.Released && ClicState == ClicStates.NONE)
            {
                clicTime.Resume();
                ClicState = ClicStates.ENGAGE;
                //Console.WriteLine("ClicState is ENGAGE \t|\tTime = " + clicTime.CurrentTime);
            }
            if (BTNState == ButtonState.Released && OldBtnState == ButtonState.Pressed && ClicState == ClicStates.ENGAGE)
            {
                ClicState = ClicStates.SIMPLE_CLIC;
                //Console.WriteLine("ClicState is SIMPLE_CLIC \t|\tTime = " + clicTime.CurrentTime);
                //Console.WriteLine("-> Just a Simple clic");
            }
            else if (BTNState == ButtonState.Released && OldBtnState == ButtonState.Pressed && ClicState == ClicStates.SIMPLE_CLIC && clicTime.CurrentTime >= DoubleClicDelay)
            {
                ClicState = ClicStates.DOUBLE_CLIC;
                //Console.WriteLine("ClicState is DOUBLE_CLIC \t|\tTime = " + clicTime.CurrentTime);
                //Console.WriteLine("\t-> Yeah, double clic");
            }
            else if (BTNState == ButtonState.Pressed && OldBtnState == ButtonState.Pressed && ClicState == ClicStates.ENGAGE)
            // When button is on ENGAGE state and the time is elapse, the state go to DRAG
            {
                if (clicTime.CurrentTime > DoubleClicDelay)
                {
                    ClicState = ClicStates.DRAG;
                    //Console.WriteLine("Ready to Drag");
                }
            }

            if (BTNState == ButtonState.Released && OldBtnState == ButtonState.Released)
            {
                if ((ClicState == ClicStates.SIMPLE_CLIC && clicTime.CurrentTime >= DoubleClicDelay) ||
                    (ClicState == ClicStates.DOUBLE_CLIC) ||
                    (ClicState == ClicStates.DRAG))
                {
                    //Console.WriteLine("Reset clic state \t|\t currentTme = " + clicTime.CurrentTime);
                    Reset();
                }
            }
        }
    }

    /// <summary>
    ///  Begin of mouse Wrrapper class
    /// </summary>

    public static class MouseWrapper
    {
        public static double DoubleClicDelay { private get; set; }
        public static MouseState CurrentState { get; private set; }
        public static Point DeltaPos { get; private set; }
        public static MouseButton LeftButton;
        public static MouseButton MiddleButton;
        public static MouseButton RightButton;
        public static Point Position { get; set; }
        public static Vector2 Velocity { get; private set; } // to implement later to measure velocity of mouse. Usefull ??
        public static int DeltaWheelValue;
        public static Vector2 FloatPosition
        {
            get
            {
                float X = Mouse.GetState().Position.X;
                float Y = Mouse.GetState().Position.Y;
                return new Vector2(X, Y);
            }
        }
        // private
        static MouseState oldState;
        
        static Point oldPosition = Point.Zero;
        static int OldWheelValue = 0;

        public static void Initialize()
        {
            CurrentState = Mouse.GetState();
            DeltaPos = Point.Zero;
            Velocity = Vector2.Zero;
            DoubleClicDelay = 0.2; // specifie the delay in second to set 2 clics as double clic
            LeftButton = new MouseButton(DoubleClicDelay);
            MiddleButton = new MouseButton(DoubleClicDelay);
            RightButton = new MouseButton(DoubleClicDelay);
            oldState = Mouse.GetState();
            //Debug.WriteLine("Mouse wrapper is now initialize");
        }

        private static void UpdateBtnState()
        {
            // Update Left Button
            if (CurrentState.LeftButton == ButtonState.Pressed)
                LeftButton.BTNState = ButtonState.Pressed;
            else
                LeftButton.BTNState = ButtonState.Released;
            // Update Middle Button
            if (CurrentState.MiddleButton == ButtonState.Pressed)
                MiddleButton.BTNState = ButtonState.Pressed;
            else
                MiddleButton.BTNState = ButtonState.Released;
            // Update Right
            if (CurrentState.RightButton == ButtonState.Pressed)
                RightButton.BTNState = ButtonState.Pressed;
            else
                RightButton.BTNState = ButtonState.Released;
        }

        public static void Update(GameTime gameTime)
        {

            // calculate the relative position since next update;
            CurrentState = Mouse.GetState();
            Position = CurrentState.Position;
            DeltaPos = Position - oldPosition;
            oldPosition = Position;
            DeltaWheelValue = CurrentState.ScrollWheelValue - OldWheelValue;

            // Update current buttons State
            UpdateBtnState();

            // Update Button click type for each button
            LeftButton.Update(gameTime);
            MiddleButton.Update(gameTime);
            RightButton.Update(gameTime);
            //Console.WriteLine("Mouse state update");
            UpdateOldStates();
        }

        public static void UpdateOldStates()
        {
            LeftButton.OldBtnState = oldState.LeftButton;
            MiddleButton.OldBtnState = oldState.MiddleButton;
            RightButton.OldBtnState = oldState.RightButton;
            OldWheelValue = Mouse.GetState().ScrollWheelValue;
            oldState = CurrentState;
            //Console.WriteLine("OldStates update");
        }
    }
}
