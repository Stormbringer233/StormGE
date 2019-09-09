using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;

namespace GUI
{
    class Label : Widget
    {
        public Color TextColor;
        protected string Text;
        public Vector2 TextMeasure { get; private set; }
        public enum H_Alignment { LEFT, CENTER, RIGHT };
        public H_Alignment H_Align;
        public enum V_Alignment { TOP, CENTER, BOTTOM};
        public V_Alignment V_Align;
        private Dictionary<States, Color> Colors;

        public Label(Widget pMaster, string pText, Vector2 pPosition)
        // Define Constructor with Vector2 positionning perameter
        {
            Master = pMaster;
            Name = "Labels";
            Position = pPosition;
            Text = pText;

            Initialize();
        }

        private Label (Widget pMaster, string pText, H_Alignment pH_Align = H_Alignment.CENTER,
                                                     V_Alignment pV_Align = V_Alignment.CENTER)
            // define an alternative constructor to positionning label with alignements compare to master
            // with default alignment
        {
            Master = pMaster;
            Name = "Labels";
            Text = pText;
            H_Align = pH_Align;
            V_Align = pV_Align;
            // Set alignment compare to master

            Initialize();
        }

        private void Initialize()
        {
            State = States.NORMAL;
            bool toMove = Master != null ? true : false;
            SetMovable(toMove);
            //widgets = new List<Widget>();
            MainFont = AssetManager.LoadFont(MltGUI.Theme.MainFont);
            // calculate bound
            TextMeasure = MainFont.MeasureString(Text);
            LabelDatas Datas = MltGUI.Theme.Widgets.Labels;
            Colors = new Dictionary<States, Color>
            {
                {States.NORMAL, ConvertFromString(Datas.NormalColor) },
                {States.MOUSEOVER, ConvertFromString(Datas.FocusColor) },
                {States.CLICKED, ConvertFromString(Datas.ClickedColor) },
                {States.FREEZE, ConvertFromString(Datas.FreezeColor) }
            };
            TextColor = Colors[State];
            // Set Bound
            Quad = new Rectangle(0, 0, (int)TextMeasure.X, (int)TextMeasure.Y);
            Bound = new Rectangle(0, 0, (int)TextMeasure.X, (int)TextMeasure.Y);
            //SetAlignment(H_Alignment.CENTER);
            Show();
        }

        private Color ConvertFromString(List<int> pColorList)
        {
            int R = pColorList[0];
            int G = pColorList[1];
            int B = pColorList[2];
            int A = pColorList[3];
            return new Color(R, G, B, A);
        }

        public void SetColors(Color pNormal = new Color(), Color pFocus = new Color(), Color pClicked = new Color(), Color pFreeze = new Color())
            // Replace all of pre-define colors in the dictionary
        {
            // change only the specify new Colors
            if (pNormal != new Color())
                Colors[States.NORMAL] = pNormal;
            if (pFocus != new Color())
                Colors[States.MOUSEOVER] = pFocus;
            if (pClicked != new Color())
                Colors[States.CLICKED] = pClicked;
            if (pFreeze != new Color())
                Colors[States.FREEZE] = pFreeze;
        }

        private void UpdateAlignement()
            // update alignement after setting new alignement
            // Note Vertical alignment is always at center of textMeasure.
        {
            //Console.WriteLine("Before setting alignment for alignement : " + H_Align + " - Quad : " + Quad);
            if (H_Align == H_Alignment.LEFT)
                Position = new Vector2(Position.X + Quad.Width / 2, Position.Y);
            else if (H_Align == H_Alignment.CENTER)
                Position = new Vector2(Position.X + Quad.Width / 2, Position.Y);
            else if (H_Align == H_Alignment.RIGHT)
                Position = new Vector2(Position.X - Quad.Width / 2, Position.Y);
            //Console.WriteLine("Position : " + Position);
        }

        public void SetAlignment(H_Alignment pNewHAlignement, V_Alignment pNewVAlignment = V_Alignment.CENTER)
        {
            H_Align = pNewHAlignement;
            UpdateAlignement();
            UpdateQuad();
            //Console.WriteLine("After setting alignment to "+ pNewHAlignement + " - Origin : " + Quad + " |new position : "+Position);
        }

        public void SetNewText(string pText)
        {
            Text = pText;
            TextMeasure = MainFont.MeasureString(Text);
            // Recalculate Quad and Bound
            Quad = new Rectangle((int)Position.X, (int)Position.Y, (int)TextMeasure.X, (int)TextMeasure.Y);
            Bound = Quad;
        }

        protected override void UpdateQuad()
        {
            // update Bound according to alignment
            Quad = new Rectangle((int)Position.X - Quad.Width / 2, (int)Position.Y - Quad.Height / 2, Quad.Width, Quad.Height);
            // Update Quad
            Bound = Quad;
            //Console.WriteLine("Bound of Label after updating : " + Bound);
        }

        public override void SetState(States pNewState)
        {
            State = pNewState;
            TextColor = Colors[State];
        }

        public override void UpdateStates()
        {
            if (State != States.FREEZE)
            {
                Point mousePos = MouseWrapper.Position;
                bool hasFocus = CollidePoint(mousePos);
                if (hasFocus && MouseWrapper.LeftButton.ClicState == MouseButton.ClicStates.NONE)
                {
                    // simply change quad
                    SetState(States.MOUSEOVER);
                }
                else if (!hasFocus)
                {
                    SetState(States.NORMAL);
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

        public override void Draw(SpriteBatch sb, GameTime gameTime)
            // The label Draw methode need to be override because of drawing different
            // Here is a DrawString methode
        {
            if (IsShown())
            {
                sb.DrawString(MainFont,
                    Text,
                    Position,
                    TextColor,
                    0,
                    new Vector2(Quad.Width / 2, Quad.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0
                    );
            }
        }

    }
}
