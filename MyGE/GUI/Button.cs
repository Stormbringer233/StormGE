using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using MyGE;

namespace GUI
{
    /**
     * CompositeWidget Button can contain
     *  - a single complete image with background, text and icon
     *  - a single background only to update text dynamically during game
     * 
     * M. Le Thiec
     * creation date : 07/10/2018
     * 
     * V : 0.00
     * V : 0.50 - 01/03/19 - L273 - add State chec when clicking on button to prevent if state is set to Freeze just after clicking
     * 
     * TODO :
     *      - add functionnality to create 
     * */

    public class Button : Widget
    {
        /**
        public Button() // TODO : must be implement better
            // simple button constructor. Initialize a button at 0,0 with main window
            // has master
            // ATTENTION : Any delegate is define, so if player clic, nothing happen
        {
            Master = null;
            Position = Vector2.Zero;
            widgets = new List<Widget>();
            State = States.NORMAL;
        }
        **/

        public delegate void ClickEvent();
        protected ClickEvent Function;
        private Dictionary<string, int> ButtonQuads = new Dictionary<string, int>
        {
            { "Button" , 4 },
            {"ToggleButton" , 6 }
        };
        private Label BtnText;
        public Vector2 Padding { get; private set; }
        public enum H_Alignment { LEFT, CENTER, RIGHT };
        public H_Alignment H_Align;
        private List<H_Alignment> TextAlignment;  // really only for horizontal alignment. vertical ... may be later !

        public Button(Widget pMaster, string pName,  Vector2 pPosition, ClickEvent pFunction, string pText = null)
            // Main constructor of button with container, relative position and delegate
        {
            // if master is null, the button's master is the scene where the button is create.
            // That's mean the button can not be moved !
            Master = pMaster;
            Position = pPosition;
            Function = pFunction;
            Name = pName;
            Initialize(pName);
            if (pText != null)
            {
                // at this point, the right position of label is not possiblie to know
                BtnText = new Label(this, pText, Vector2.Zero);
                BtnText.SetMovable(true);
                Add(BtnText);
            }
        }

        private void Initialize(string pName)
            // common Initialize. Used by all of constructors.
            // Initialize all of datas from Json Theme file
        {
            Show();
            bool toMove = Master != null ? true : false;
            SetMovable(toMove);
            TextAlignment = new List<H_Alignment> { H_Alignment.CENTER, H_Alignment.CENTER };
            Padding = new Vector2(5f, 5f); // add padding around button for widget inside positionning
            ButtonsDatas Datas = MltGUI.Theme.Widgets.GetButtonDatas(pName);
            if (Datas == null)
            {
                Console.WriteLine("Unable to find widget <" + pName + ">. Using default widget instead.");
                Datas = MltGUI.Theme.Widgets.GetButtonDatas("default");
            }

            Image = AssetManager.LoadImage(SheetName);
            Quads = new List<Rectangle>();
            widgets = new List<Widget>();
            State = States.NORMAL;
            // bound datas recover
            Bound = new Rectangle(0, 0, Datas.Bound.Width, Datas.Bound.Height);
            OriginBound = new Rectangle(0, 0, Datas.Bound.Width, Datas.Bound.Height);
            // build quads
            int Xinit = Datas.Xinit;
            int Yinit = Datas.Yinit;
            int width = Datas.Width;
            int height = Datas.Height;
            // TODO : consider calculate quads with padding and spacing values
            int padding = MltGUI.Theme.Widgets.Padding; // the distance with border of sheet (usually 0)
            int spacing = MltGUI.Theme.Widgets.Spacing; // the space between 2 images (usually 0)
            if (Datas.Anchor.ToUpper() == "CENTER")
            {
                Xinit -= width / 2;
                Yinit -= height / 2;
            }
            int x = Xinit;
            int y = Yinit;
            // build quads for manage animations from widget sheet
            // ATTENTION : the quad 2 & 3 are the sames because it's the same image for CLICKED and ACTIVATE state
            for (int i = 0; i < Datas.TotalAnimations; i++)
            {
                // in case of animation if not draw with 4 states (normal, mouseover, clicked and freeze)
                // it's not a normality
                if (Datas.TotalAnimations < ButtonQuads[Datas.WidgetType])
                {
                    if (i == Datas.TotalAnimations)
                    {
                        x = Xinit;
                        y = Yinit;
                    }
                }
                Rectangle quad = new Rectangle(x, y, width, height);
                Quads.Add(quad);
                x += width;
            }
            UpdateQuad(); // define 1st rectangle as initial quad
            //Console.WriteLine("Create button "+Datas.Name+" at : " + Position);
        }


        // ****************************************************************************
        public void Click()
            // Launch the binding function
        {
            // same as if (Function != null) Function();
            Function?.Invoke(); // invoke Function if Function != null
        }

        private Vector2 GetCenterPosition()
        {
            Vector2 center = Vector2.Zero;
            if (Master != null)
            {
                // calculate the top/left position of the master of button. Usually a window or a frame
                int left = (int)Master.Position.X - Master.Quad.Width / 2;
                int top = (int)Master.Position.Y - Master.Quad.Height / 2;
                center = new Vector2(Position.X + left, Position.Y + top);
            }
            else
            {
                // center on the button
                center = Position;
            }
            return center;
        }

        public override void Add(Widget pElement)
        {
            base.Add(pElement);
            // update placement compareto button center
            pElement.Position = GetCenterPosition();
        }

        public void SetNewText(string pText)
        {
            // For change text, the button must have some widgets inside it
            if ((widgets?.Count > 0))
            {
                // and of course, we need a label
                foreach (Widget w in widgets)
                {
                    if (w.Name.ToUpper() == "LABELS")
                    {
                        var tempLabel = (Label)w;
                        tempLabel.SetNewText(pText);
                        var currentAlign = TextAlignment[0]; // store current alignment
                        SetAlignment(w.Name, H_Alignment.CENTER); // realigne to center with the new label lenght
                        SetAlignment(w.Name, currentAlign); // restore the stored alignment
                    }
                }
            }
        }

        public void SetAlignment(string pWidgetName, H_Alignment pAlign)
            // must be change. WRONG behavior
        {
            foreach(Widget w in widgets)
            {
                if (w.Name.ToUpper() == "LABELS")
                {
                    var label = (Label)w;
                    var textLength = label.TextMeasure.X;

                    if (pAlign == H_Alignment.CENTER)
                        // place the center of the label on the center of button
                    {
                        label.Position = Position;
                    }
                    else if (pAlign == H_Alignment.LEFT)
                        // place the left side of the label at Padding pixels of the left side of the button
                        // that's mean the center of the label will be moved consequently
                    {
                        // if the button is inside a Master (usually a window or a frame) and
                        // if button has been add into Master at this moment, then label position must be reset.
                        if (Master != null && Master.widgets.Contains(this))
                            label.Position = Position; // reset current position
                        label.Position = new Vector2(label.Position.X - Quad.Width / 2 + textLength / 2 + Padding.X, label.Position.Y);
                    }
                    else if (pAlign == H_Alignment.RIGHT)
                    {
                        if (Master != null && Master.widgets.Contains(this))
                            label.Position = Position;
                        label.Position = new Vector2(label.Position.X + Quad.Width / 2 - textLength / 2 - Padding.X, label.Position.Y);
                    }
                    TextAlignment[0] = pAlign;
                }
                else if (w.Name.ToUpper() == "ICONS")
                {
                    // TODO later
                    Console.WriteLine("align icon from center to " + pAlign);
                }
            }
        }

        public override void UpdateStates()
            // Update the state of the widget
        {
            /** The behavior of the button is :
                * When it take focus, button aspect change
                * When it is clicked, button aspect change, but action is not engage
                * when button is released AND focus is true, the action is engage
                * when button is released and focus is lost, button return to normal state
                * */
            // We don't try to update button state if it is freeze
            if (State != States.FREEZE)
            {
                Point mousePos = MouseWrapper.Position;
                bool hasFocus = CollidePoint(mousePos);
                //Debug.WriteLineIf(State == States.CLICKED, "Focus : " + hasFocus + "\tMouse button is " + MouseWrapper.LeftButton.ClicState);
                //Debug.WriteLine("Focus : " + hasFocus + "\t|\tMouse button is " + MouseWrapper.LeftButton.ClicState + "\t|\tState = " + State);

                if (hasFocus && MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.NONE)
                {
                    // simply change quad
                    State = States.MOUSEOVER;
                    //Console.WriteLine("Mouse is over button - State = " + State);
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.SetState(States.MOUSEOVER);
                        }
                    }
                }
                if ((MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.ENGAGE || MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.DRAG) 
                        && State == States.MOUSEOVER)
                {
                    // the button is simply push
                    State = States.CLICKED;
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.SetState(States.CLICKED);
                        }
                    }
                    //Console.WriteLine("button state is clicked");
                }
                else if (State == States.CLICKED && MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.SIMPLE_CLIC)
                {
                    Console.WriteLine("throwing action of button");
                    Click();
                    if (State == States.CLICKED) // To prevent if just after clicking, the button has been state to FREEZE
                        State = States.NORMAL;
                }
                else if (!hasFocus)
                {
                    State = States.NORMAL;
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.SetState(States.NORMAL);
                        }
                    }
                }
                UpdateQuad();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsShown())
            {
                UpdateStates();
                if (UnderMove)
                {
                    Point delta = MouseWrapper.DeltaPos;
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.Move(delta);
                            //w.Update(gameTime);
                        }
                    }
                }
                else
                {
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.UnderMove = false;
                        }
                    }
                }
            }
        }
    }
} // end of NameSpace GUI
