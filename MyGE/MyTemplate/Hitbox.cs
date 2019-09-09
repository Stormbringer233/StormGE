using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class Hitbox
    {
        /// <summary>
        /// Represent a simple 2D box for manage hitbox center of the position of the center of the box
        /// 
        /// M. Le Thiec
        /// Creation date : 04/11/2018
        /// 
        /// </summary>
        public Vector2 Position { get; set; } // The center of the box
        public Rectangle Bound { get; private set; } // the bound of box
        private bool toUpdate;

        public Hitbox()
        {
            Position = Vector2.Zero;
            Bound = new Rectangle();
            toUpdate = false;
        }

        public Hitbox(Hitbox pBox)
            // Dublicate box
        {
            Position = new Vector2(pBox.Position.X, pBox.Position.Y);
            Bound = new Rectangle(pBox.Bound.X, pBox.Bound.Y, pBox.Bound.Width, pBox.Bound.Height);
            toUpdate = pBox.toUpdate;
        }

        public Hitbox(Vector2 pPosition, Rectangle pBound)
            // create new hitbox center on the position
        {
            Position = pPosition;
            Bound = pBound;
            toUpdate = true;
        }

        public Hitbox(Rectangle pBound)
            // create new hitbox from simple bound.
            // The center position is deducted from center of bound
        {
            if (pBound != new Rectangle())
            {
                Bound = pBound;
                Point center = pBound.Center;
                Position = new Vector2(center.X, center.Y);
                toUpdate = true;
            }
            else
            {
                Position = Vector2.Zero;
                Bound = new Rectangle();
                toUpdate = false;
            }
        }

        public void SetBound(int pX, int pY, int pWidth, int pHeight)
            // Update bound from top left corner, width and height
            // after that, it's necessary to update position
        {
            Bound = new Rectangle(pX, pY, pWidth, pHeight);
            if (Bound != new Rectangle())
            {
                Point center = Bound.Center;
                Position = new Vector2(center.X, center.Y);
            }
            else
                toUpdate = false;
        }

        private void UpdateBox()
            // update the coordinate of the box according to the position (center of the box)
        {
            Bound = new Rectangle((int)Position.X - Bound.Width / 2, (int)Position.Y - Bound.Height / 2, Bound.Width, Bound.Height);
        }

        public void Move(Vector2 pPosition)
            // Update position of the box according to the position of the sprite
        {
            if (toUpdate)
            {
                Position = Position + pPosition;
                UpdateBox();
                //Console.WriteLine("pos : Position = " + Position + " | Bound = " + Bound);

            }
        }

        public void Move(Vector2 pPosition, SpriteEffects pEffect)
        {
            if (toUpdate)
            {
                Move(pPosition);
                Mirror(pPosition, pEffect);
            }
        }

        public void Mirror(Vector2 pSymetricPosition, SpriteEffects pEffect)
            // invert coordinate according to effect with Sysmetric position as center of mirror
        {
            if (toUpdate)
            {
                if (pEffect == SpriteEffects.FlipHorizontally)
                {
                    float deltaX = Position.X - pSymetricPosition.X;
                    Position = new Vector2(pSymetricPosition.X - deltaX, Position.Y);
                    UpdateBox();
                    //Console.WriteLine("mirrot pos : Position = " + Position + " | Bound = " + Bound);
                }
            }
        }

        public bool Contains(Hitbox pBox)
        {
            return Bound.Contains(pBox.Bound);
        }

        public bool Contains(List<Hitbox> pListBox)
        {
            if (Bound != new Rectangle())
            {
                foreach(Hitbox box in pListBox)
                {
                    if (Bound.Contains(box.Bound))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Intersect(Hitbox pBox)
        {
            //Console.WriteLine("Compare hitbox : " + Bound + " with " + pBox.Bound);
            return Bound.Intersects(pBox.Bound);
        }

        public bool Intersect(List<Hitbox> pListBox)
        {
            //Console.WriteLine("into Intersect(List) - hurdtboxes.Count = " + pListBox.Count);
            foreach (Hitbox box in pListBox)
            {
                if (Bound.Intersects(box.Bound))
                {
                    //Console.WriteLine("Compare hitbox : " + Bound + " with " + box.Bound);
                    return true;
                }
            }
            return false;
        }
    }
}
