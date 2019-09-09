using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GUI
{
    /* Base class for all of graphic elements
     * 
     * Master is the main container of the widget. if Master is null, then, the
     * container is the main window and the widget will be fixed.
     * Otherwise, the position of the widget will be update with the container moving
     * 
     * M. Le Thiec
     * 07/10/2018
     * 
     * V : 0.00
     **/

    public abstract class Widget : IComposite
    {
        public Widget Master; //  define the master of the composite to refere to it.
        public List<Widget> widgets;

        public string Name { get; protected set; }
        public Color WidgetColor = Color.White;
        //public float Scale = 1f;
        public Vector2 Position { get; set; }
        public Rectangle Bound { get; set; }
        public bool ToShow { get; set; }
        protected bool Movable { get; set; }
        public List<Rectangle> Quads; // list of rects for manage animation
        public Rectangle Quad { get; set; } // current frame of animation
        protected Rectangle OriginBound;
        protected string SheetName = MltGUI.Theme.Widgets.SheetName;
        protected SpriteFont MainFont;
        protected SpriteFont BigTitleFont;
        protected SpriteFont MediumTitlefont;
        protected SpriteFont SmallTitleFont;
        // As windows have her own classes derived from this one, Image must be load in the concrete class like Button for example
        protected Texture2D Image;
        // IMPORTANT : file definition and sprite sheet MUST FOLLOW the same order of States
        public enum States { NORMAL, MOUSEOVER, CLICKED, FREEZE}
        public States State;
        public bool UnderMove;


        // ********************** Composite behaviors ***********************

        public virtual void Add(Widget pElement)
        {
            if (!widgets.Contains(pElement))
                widgets.Add(pElement);
            //After each object must define the placement of the component compared to it

        }

        public Widget GetChild(int pIndex)
        {
            if (pIndex < widgets.Count)
                return widgets[pIndex];
            return null;
        }

        public void Remove(Widget pElement)
        {
            if (widgets.Contains(pElement))
                widgets.Remove(pElement);
        }

        // ******************************* Widget fonctionnalities **********************************

        public abstract void UpdateStates();

        protected virtual bool CollidePoint(Point pPosition)
            // Check if the point is colliding (is under) the bounding box
        {
            //Console.WriteLine("bound = " + Bound + " | mouse position = " + pPosition);
            if (Bound.Contains(pPosition))
            {
                //Console.WriteLine(pPosition + " is under { " + Bound.X + ", " + Bound.Y + ", " + (Bound.X + Bound.Width) + ", " + (Bound.Y + Bound.Height) + " }");
                return true;
            }
            return false;
        }

        protected virtual void UpdateBound()
            // Update the bounding box corresponding to the image position
        {
            // Set bound calculation at this place ONLY if widget Width and Height don't change ! Be carefull
            int w = (int)(OriginBound.Width * MltGUI.Scale);
            int h = (int)(OriginBound.Height * MltGUI.Scale);
            Bound = new Rectangle((int)Position.X - w / 2, (int)Position.Y - h / 2, w, h);
        }

        protected virtual void UpdateQuad()
            // update the currentQuad corresponding to the widget state
        {
            //Console.WriteLine("current state is : " + State + "\t int -> " + (int)State);
            int frame = ((int)State <= Quads.Count - 1) ? (int)State : 0;
            Quad = new Rectangle(Quads[frame].X, Quads[frame].Y, Quads[frame].Width, Quads[frame].Height);
            UpdateBound();
        }

        public void Move(float pDX, float pDY)
            // move the widget proportionally to DX and DY
        {
            //Console.WriteLine("Moveing a widget !");
            if (Movable)
            {
                Position = new Vector2(Position.X + pDX, Position.Y + pDY);
                //Console.WriteLine("new position of widget "+Name+" is : " + Position);
            }
        }

        public void Move(Point pNewPosition)
            // Overload for mouse position variable type
        {
            Move(pNewPosition.X, pNewPosition.Y);
        }

        public void MoveAt(float pX, float pY)
            // move the widget to another position at X and Y
        {
            if (Movable)
            {
                Position = new Vector2(pX, pY);
            }
        }

        public void MoveAt(Point pNewPosition)
            // Overload for mouse position variable type
        {
            MoveAt(pNewPosition.X, pNewPosition.Y);
        }

        public virtual void Show()
        {
            ToShow = true;
        }

        public virtual void Hide()
        {
            ToShow = false;
        }

        public bool IsShown()
        {
            return ToShow;
        }

        public virtual void SetState(States pNewState)
            // Force the state of the widget (usaully to turn of widget => State become FREEZE)
        {
            State = pNewState;
            if ((widgets?.Count > 0))
            {
                foreach (Widget w in widgets)
                {
                    w.SetState(pNewState);
                }
            }
            UpdateQuad();
        }

        public void SetMovable(bool pNewMovableState)
        {
            Movable = pNewMovableState;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (IsShown())
            {
                sb.Draw(Image,
                    Position,
                    Quad,
                    WidgetColor,
                    0f,
                    new Vector2(Quad.Width / 2, Quad.Height / 2),
                    MltGUI.Scale,
                    SpriteEffects.None,
                    0f
                    );
                if ((widgets?.Count > 0))
                {
                    foreach (Widget w in widgets)
                    {
                        w.Draw(sb, gameTime);
                    }
                }
            }
        }
    }
}
