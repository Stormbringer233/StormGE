using System;
using static System.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MyTemplate;

namespace GUI
{
    /**
     * Basic class that implement a window. Typically, a window is a frame that contains somes widgets, probably
     * some buttons and other things to interact with the player
     * A window is composed with 2 frames :
     *      - 1st : the main window container
     *      - 2nd : the top bar with some buttons for example : close window button. The top bar allow to move the window
     *      
     * M. Le Thiec
     * creation date : 21/10/2018
     * 
     * V : 0.00
     * 
     * Last update : 25/10/2018
     * separate the window creation in 2 entities : The TopBar and the MainFrame. This design allow to manage each part
     * of the window separatly with the same code design (CompositeWidget)
     * 
     * */

    public abstract class Window : Widget
        // Define a window basic class that manage the main behavior. A window is define by 2 main elements :
        //      - the TopBar that contain buttons like close window or minimize
        //      - the MainFrame that contain the mains widgets defines for driving the window's fonctionnalities
        // Note : the position of the window is the center of the TopBar by default.
    {
        protected class TopBarComponent : Widget
            // Define an inner class for top bar. 
        {

            public TopBarComponent(string pName, Vector2 pPosition, bool pUnderMove)
                // pName : allow to acces to the datas of windows
                // pPosition : the center of the entire window
            {
                widgets = new List<Widget>();
                Movable = true;
                WindowsCompleteDatas Datas = MltGUI.Theme.Windows.GetWindowsDatas(pName);
                if (Datas == null)
                {
                    WriteLine("Unable to find widget <" + pName + ">. Using default widget instead.");
                    Datas = MltGUI.Theme.Windows.GetWindowsDatas("default");
                }
                Position = pPosition;
                Image = AssetManager.LoadImage(MltGUI.Theme.Windows.SheetName);
                Quad = new Rectangle(Datas.TopBar.Image.Xinit, Datas.TopBar.Image.Yinit,
                                     Datas.TopBar.Image.Width, Datas.TopBar.Image.Height);
                int topX = ((int)Position.X + Datas.TopBar.Bound.Xinit) - Quad.Width / 2;
                int topY = ((int)Position.Y + Datas.TopBar.Bound.Yinit) - Quad.Height / 2;
                int topW = Datas.TopBar.Bound.Width;
                int topH = Datas.TopBar.Bound.Height;
                Bound = new Rectangle(topX, topY, topW, topH);
                UnderMove = pUnderMove;
            }

            public override void Add(Widget pElement)
            {
                // add an element to the container
                base.Add(pElement);
                // place on the element compare to the topbar
                // Note that element must be place compare to th top left reférence of the TopBar
                int left = (int)Position.X - Quad.Width / 2;
                int top = (int)Position.Y - Quad.Height / 2;
                pElement.Position = new Vector2(left + pElement.Position.X, top + pElement.Position.Y);
                //WriteLine("Widget position is now : " + pElement.Position);
                
            }

            public override void Hide()
            {
                UnderMove = false;
                base.Hide();
            }

            public void UpdateBounds(Point pDeltaPosition)
            {
                Bound = new Rectangle(Bound.X + pDeltaPosition.X, Bound.Y + pDeltaPosition.Y, Bound.Width, Bound.Height);
            }

            public override void UpdateStates()
            {
                throw new NotImplementedException();
            }

            public override void Update(GameTime gameTime)
            {
                if (IsShown())
                {
                    if (!UnderMove)
                    {
                        bool hasFocus = CollidePoint(MouseWrapper.Position);
                        if (hasFocus && MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.NONE)
                        {
                            if ((widgets?.Count > 0))
                            {
                                foreach (Widget w in widgets)
                                {
                                    if (w.State == States.MOUSEOVER)
                                    {
                                        State = States.NORMAL;
                                    }
                                    else
                                        State = States.MOUSEOVER;
                                }
                            }
                            else
                                State = States.MOUSEOVER;
                            //WriteLine("win state is MOUSEOVER |\thasFocus = " + hasFocus);
                        }
                        else if (State == States.MOUSEOVER && (MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.ENGAGE ||
                             MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.DRAG))
                        {
                            UnderMove = true;
                            //WriteLine("UnderMove is now True");
                        }
                        else
                        {
                            State = States.NORMAL;
                        }
                    }
                    else
                    {
                        Point delta = MouseWrapper.DeltaPos;
                        Move(delta);
                        if ((widgets?.Count > 0))
                        {
                            foreach (Widget w in widgets)
                            {
                                w.UnderMove = true;
                                w.Move(delta);
                            }
                        }
                        UpdateBounds(delta);
                        if (MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.NONE && UnderMove == true)
                        {
                            UnderMove = false;
                            State = States.NORMAL;
                            if ((widgets?.Count > 0))
                            {
                                foreach (Widget w in widgets)
                                {
                                    w.UnderMove = false;
                                }
                            }
                            //WriteLine("reset window to NORMAL state");
                        }
                    }
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.Update(gameTime);
                        }
                    }
                }
            }
        }

        // ************************************** END of TopBar *****************************************

        protected class MainFrameComponent : Widget
        {
            private Widget TopBar;
            private readonly int dx; // dx & dy are space between TopBar and Mainframe.
            private readonly int dy; // These datas are strored in Theme.json

            public MainFrameComponent(string pName, Vector2 pPosition, bool pUnderMove, TopBarComponent pTopBar)
            {
                widgets = new List<Widget>();
                TopBar = pTopBar;
                UnderMove = pUnderMove;
                Movable = true;
                WindowsCompleteDatas Datas = MltGUI.Theme.Windows.GetWindowsDatas(pName);
                if (Datas == null)
                {
                    WriteLine("Unable to find widget <" + pName + ">. Using default widget instead.");
                    Datas = MltGUI.Theme.Windows.GetWindowsDatas("default");
                }
                Image = AssetManager.LoadImage(MltGUI.Theme.Windows.SheetName);
                Quad = new Rectangle(Datas.MainFrame.Xinit, Datas.MainFrame.Yinit,
                                     Datas.MainFrame.Width, Datas.MainFrame.Height);
                dx = MltGUI.Theme.Windows.MainFrameSpaceX;
                dy = MltGUI.Theme.Windows.MainFrameSpaceY;
                Position = new Vector2(TopBar.Position.X + dx, TopBar.Position.Y + dy + TopBar.Quad.Height / 2 + Quad.Height / 2);

                int topX = ((int)Position.X + Datas.MainFrame.Xinit) - Quad.Width / 2;
                int topY = ((int)Position.Y + Datas.MainFrame.Yinit) - Quad.Height / 2;
                int topW = Datas.MainFrame.Width;
                int topH = Datas.MainFrame.Height;
                Bound = new Rectangle(topX, topY, topW, topH);
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

            private void UpdateBounds(Point pDeltaPosition)
            {
                Bound = new Rectangle(Bound.X + pDeltaPosition.X, Bound.Y + pDeltaPosition.Y,
                                       Bound.Width, Bound.Height);
            }

            public override void UpdateStates()
            {
                throw new NotImplementedException();
            }

            public override void Update(GameTime gameTime)
            {
                if (UnderMove)
                {
                    Point delta = MouseWrapper.DeltaPos;

                    Move(delta);
                    //UpdateBounds(delta);
                    if ((widgets?.Count > 0))
                    {
                        foreach (Widget w in widgets)
                        {
                            w.UnderMove = true;
                            w.Move(delta);
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
                if ((widgets?.Count > 0))
                {
                    foreach (Widget w in widgets)
                    {
                        w.Update(gameTime);
                    }
                }
            }

        }

        // ************************************** END of MainFrame *****************************************

        protected TopBarComponent TopBar;
        protected MainFrameComponent MainFrame;

        public Window(Widget pMaster, string pName, Vector2 pPosition)
        {
            Master = pMaster;
            Position = pPosition;
            TopBar = new TopBarComponent(pName, pPosition, false);
            MainFrame = new MainFrameComponent(pName, pPosition, false, TopBar);
            Initialize(pName);
        }

        private void Initialize(string pName)
        {
            TopBar.Show();
            MainFrame.Show();
            SetMovable(true);
        }

        public override void Add(Widget pElement)
        {
            var master = pElement.Master;
            master.Add(pElement);
            //base.Add(pElement);
        }

        public override void Hide()
        {
            MainFrame.Hide();
            TopBar.Hide();
        }

        public override void UpdateStates()
        {
        }

        public override void Update(GameTime gameTime)
        {
            MainFrame.UnderMove = TopBar.UnderMove;
            MainFrame.Update(gameTime);
            TopBar.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            MainFrame.Draw(sb, gameTime);
            TopBar.Draw(sb, gameTime);
        }
    }
}
