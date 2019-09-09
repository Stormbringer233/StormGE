using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class MouseDrivenCamera : Camera2D
        /// These camera is design to be manage with the mouse.
        /// It's usually a camera for 2D STrategy or managment game
        ///
        /// Basics Behaviors :
        /// a single region for the entire viewport wide
        /// scrolling by relative position of the mouse into the viewport
        /// The scrolling is progressive. The max velocity is when the mouse cursor is near fro sides of viewport
        /// The mouse trigged the scrolling only if it's inside the viewport
        /// Zoom in and out with the wheel
        /// When zooming, the area scroll to the center of viewport, the mouse is set to center too
        /// 
    {
        // Private attributs
        private float scrollThreshold;
        private float k;
        private float CurrentMaxVelocity;
        // Protected attributs

        // Public attibuts
        public enum StaticBounds { FULL, HALF, NINETY, HEIGHTY}
        public StaticBounds CurrentBounds;
        public float ZoomSpeed;
        public float Max_Velocity;
        public Vector2 MouseNormalizedPosition { get; private set; }
        public Vector2 MouseWorldPosition
        {
            get
            {
                return Vector2.Transform(MouseWrapper.FloatPosition - new Vector2(Viewport.X, Viewport.Y), Matrix.Invert(Transform));
            }
        }


        public MouseDrivenCamera(Viewport pViewport) : base(pViewport)
        {
            k = 1;
            Initialize();
        }

        public MouseDrivenCamera(Viewport pViewport, Rectangle pRegion) : base(pViewport, pRegion)
        {
            k = 1;
            Initialize();
        }

        public MouseDrivenCamera(Viewport pViewport, Rectangle pRegion, float pScrollThresold) : base(pViewport, pRegion)
        {
            scrollThreshold = (pScrollThresold > 0) ? pScrollThresold : 0;
            k = 1;
            Initialize();
        }

        public MouseDrivenCamera(Viewport pViewport, Rectangle pRegion, StaticBounds pScrollBound) : base(pViewport, pRegion)
        {
            k = 1;
            switch (pScrollBound)
            {
                case StaticBounds.FULL:
                    k = 1;
                    break;
                case StaticBounds.HALF:
                    k = 0.5f;
                    break;
                case StaticBounds.HEIGHTY:
                    k = 0.8f;
                    break;
                case StaticBounds.NINETY:
                    k = 0.9f;
                    break;
                default:
                    k = 1;
                    break;
            }
            Initialize();
        }

        private void Initialize()
        {
            ZoomSpeed = 0.0075f;
            scrollThreshold = (scrollThreshold <= 0) ? 0.8f : scrollThreshold;
            Debug.WriteLine("scrollthresold : " + scrollThreshold);
            Max_Velocity = 5; // the velocity is set to the same value for X and Y
            CurrentMaxVelocity = Max_Velocity;
            Velocity = Vector2.Zero;
            Debug.WriteLine("viewport bound : " + Viewport.Bounds);
            SetScrollBound(new Rectangle(0, 0, (int)(Viewport.Width * k), (int)(Viewport.Height * k)));
            MouseNormalizedPosition = Vector2.Zero;
        }

        public void Zoom()
        {
            if (MouseWrapper.DeltaWheelValue > 0)
                Scale *= Math.Abs(MouseWrapper.DeltaWheelValue) * ZoomSpeed;
            else if (MouseWrapper.DeltaWheelValue < 0)
                Scale /= Math.Abs(MouseWrapper.DeltaWheelValue) * ZoomSpeed;
            // Update velocity toalways scroll at the same velocity
            CurrentMaxVelocity = Max_Velocity * Scale;
        }

        private bool IsUnderScrollBounds(Vector2 pMousePosition)
            // Return true is mouse cursor is under the scrollbounds.

        {
            // Do not forget to add viewport offset position compared to 0, 0 screen position
            if (pMousePosition.X > (Viewport.X + ScrollBound.Left) && pMousePosition.X < (Viewport.X + ScrollBound.Right) &&
                pMousePosition.Y > (Viewport.Y + ScrollBound.Top) && pMousePosition.Y < (Viewport.Y + ScrollBound.Bottom))
            {
                return true;
            }
            return false;
        }

        private Vector2 NormalizePositionComparedToCenter(Vector2 pMousePosition)
            // return the normalization of the mouse position compared to the center of viewport
            // ATTENTION : the mouse is always reference to the main window
            // reference scrollbounds :
            // -1,-1 ------------------ 1,-1
            //       |        |0,0    |
            //       |--------¤-------|
            //       |        |       |
            // -1, 1 ------------------ 1, 1
        {
            float normX = 0;
            float normY = 0;
            if (IsUnderScrollBounds(pMousePosition))
            {
                // calculate mouse position compared to viewport center
                float Centerx = pMousePosition.X - Viewport.X - ViewportCenter.X + ScrollBoundOffset.X;
                float Centery = pMousePosition.Y - Viewport.Y - ViewportCenter.Y + ScrollBoundOffset.Y;
                normX = Centerx / (ScrollBound.Width / 2) / Scale;
                normY = Centery / (ScrollBound.Height / 2) / Scale;
                //Debug.WriteLine("Centerx, centery = "+Centerx + " , "+Centery + "\tnormX, normY = " + normX + " , " + normY);
            }
            return new Vector2(normX, normY);
        }

        protected override void Move(Vector2 pDeltaMove)
        {
            var oldPosition = CameraPosition;
            CameraPosition += pDeltaMove;
            if (!IsUnderWorld())
            {
                CameraPosition = oldPosition;
            }
        }

        public override void Update(GameTime gameTime)
        {
            MouseWrapper.Update(gameTime);
            if (ScrollOn)
            {
                MouseNormalizedPosition = NormalizePositionComparedToCenter(MouseWrapper.FloatPosition);
                //Debug.WriteLine("normalize position = " + MouseNormalizedPosition);
                float xSpeed = 0;
                float ySpeed = 0;
                if (Math.Abs(MouseNormalizedPosition.X) > Math.Abs(scrollThreshold))
                {
                    if (MouseNormalizedPosition.X > 0)
                        xSpeed = CurrentMaxVelocity * ((MouseNormalizedPosition.X - scrollThreshold) / (1 - scrollThreshold));
                    else
                        xSpeed = CurrentMaxVelocity * ((MouseNormalizedPosition.X + scrollThreshold) / (1 - scrollThreshold));

                }
                if (Math.Abs(MouseNormalizedPosition.Y) > Math.Abs(scrollThreshold))
                {
                    if (MouseNormalizedPosition.Y > 0)
                        ySpeed = CurrentMaxVelocity * ((MouseNormalizedPosition.Y - scrollThreshold) / (1 - scrollThreshold));
                    else
                        ySpeed = CurrentMaxVelocity * ((MouseNormalizedPosition.Y + scrollThreshold) / (1 - scrollThreshold));
                }
                Velocity = new Vector2(xSpeed, ySpeed);
                Move(Velocity);
            }
            if (MouseWrapper.DeltaWheelValue != 0)
                Zoom();
            MouseWrapper.UpdateOldStates();
        }

        public override void Set(SpriteBatch sb)
        {
            base.Set(sb);
        }

        public override void Unset(SpriteBatch sb)
        {
            base.Unset(sb);
        }
    }
}
