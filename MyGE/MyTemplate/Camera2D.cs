using Content.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public abstract class Camera2D
    {
        // private Attributes
        private float scale;

        // protected
        protected bool OnShake;
        protected Vector2 CameraOffset;
        protected Vector2 ScrollBoundOffset;
        protected Rectangle ScrollBoundZone;
        protected Vector2 OldCameraPosition;
        protected Vector2 Velocity;
        protected bool Symetrize;
        protected Directions CurrentYDirection;
        protected Vector2 OriginalOffset;
        protected int OffsetK;
        protected Dictionary<string, ShaderEffect> ListEffects;
        protected ShaderEffect CurrentEffect;
        // public
        public enum Directions { NONE, LEFT, TOP, RIGHT, BOTTOM }
        public enum ScrollStates { FIX, ONSCROLLCAMERA, ONCAMERATWEEN, TELEPORT, SYMETRIZE }
        public ScrollStates ScrollState;

        public Viewport Viewport { get; protected set; }
        public Viewport DefaultViewport { get; private set; }

        public Vector2 CameraPosition { get; set; } // same as sprite position. By default, the center of the viewport and the center of scrollBound
        public float AngleZ { get; set; }
        public Point RotateCenter { get; set; }
        public Vector2 ScaleRange;
        public Rectangle WorldBound { get; set; } // the dimension of the entire world
        public Rectangle ScrollBound { get; set; } // the dimension of the scroll trigger
        public Directions CurrentXDirection { get; set; }
        public bool ScrollOn { get; set; }
        public bool IntoScrollBound;

        // Properties definitions
        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(-CameraPosition.X, -CameraPosition.Y, 0) *
                       Matrix.CreateRotationZ(AngleZ) *
                       Matrix.CreateScale(Scale) *
                       Matrix.CreateTranslation(Viewport.Width / 2 + CameraOffset.X, Viewport.Height / 2 + CameraOffset.Y, 0);
                // *
                       //Matrix.CreateTranslation(RotateCenter.X, RotateCenter.Y, 0);
            }
        }

        public float Scale
        {
            get
            {
                    return scale;
            }
            set
            {
                if (value < ScaleRange.X) scale = ScaleRange.X; // min scale
                else if (value > ScaleRange.Y) scale = ScaleRange.Y; // max scale
                else scale = value;
            }

        }

        public Point ViewportCenter
        {
            get { return new Point(Viewport.Width / 2, Viewport.Height / 2); }
        }

        // Implentation
        public Camera2D(Viewport pViewport)
        {
            Viewport = pViewport;
            Initialize();
        }

        public Camera2D(Viewport pViewport, Rectangle pRegion)
        {
            Viewport = new Viewport(pRegion);
            Initialize();
        }

        private void Initialize()
        {
            DefaultViewport = new Viewport(Viewport.Bounds);
            WorldBound = Viewport.Bounds;
            ScrollBound = Viewport.Bounds;
            ScaleRange = new Vector2(0.1f, 4);
            Scale = 1;
            CameraOffset = Vector2.Zero;
            OriginalOffset = Vector2.Zero;
            CameraPosition = new Vector2(ViewportCenter.X, ViewportCenter.Y);
            OldCameraPosition = CameraPosition;
            Velocity = Vector2.Zero;
            ScrollBoundOffset = Vector2.Zero;
            Symetrize = false;
            CurrentXDirection = Directions.RIGHT;
            ScrollOn = true;
            ListEffects = new Dictionary<string, ShaderEffect>()
            {
                {"NONE", new DefaultEffect() },
                {"FADEOUT", new FadeOut() }
            };
            CurrentEffect = ListEffects["NONE"]; // load simple basic effect
        }

        public void AddEffect(string pEffectName, ShaderEffect pShaderEffect)
        {
            ListEffects.Add(pEffectName.ToUpper(), pShaderEffect);
        }

        public void SelectEffect(string pEffectName)
        {
            if (ListEffects.ContainsKey(pEffectName.ToUpper()))
            {
                CurrentEffect = ListEffects[pEffectName.ToUpper()];
            }
        }

        public void Reset()
        {
            Scale = 1;
            AngleZ = 0;
        }

        public Vector2 GetVelocity()
        {
            return Velocity;
        }

        public virtual void SetScrollBound(Rectangle pBound)
            // Center ScrollBound at the center around Position
            // Must be change later
            // pBound.X = distance from viewport center X
            // pBound.Y = distance from viewport center Y
            // pBound.Width = ScrollBound.Width
            // pBound.Height = ScrollBound.Height
        {
            CameraOffset = new Vector2(pBound.X, pBound.Y);
            OriginalOffset = new Vector2(pBound.X, pBound.Y);
            CameraPosition = new Vector2(Viewport.Width / 2 + CameraOffset.X, Viewport.Height / 2 + CameraOffset.Y);
            OldCameraPosition = CameraPosition;
            ScrollBound = new Rectangle((int)(CameraPosition.X - pBound.Width / 2 + ScrollBoundOffset.X),
                                        (int)(CameraPosition.Y - pBound.Height / 2 + ScrollBoundOffset.Y),
                                        pBound.Width, pBound.Height);
            Debug.WriteLine("scrollbounds : " + ScrollBound);

        }

        public void SetScrollBound(Rectangle pBound, Vector2 pPositionOffset)
            // pPosition Offset manage the offset with the camera position
        {
            //Debug.WriteLine("pPositionOffset = " + pPositionOffset);
            ScrollBoundOffset = pPositionOffset;
            SetScrollBound(pBound);
        }

        protected bool IsUnderWorld()
        {
            //Debug.WriteLine("IsUnderWorld Y bottom : " + (WorldBound.Top + RotateCenter.Y) / Scale);
            if (CameraPosition.X >= (WorldBound.Left + Viewport.Width / 2 + CameraOffset.X) / Scale &&
                CameraPosition.X <= (WorldBound.Right - Viewport.Width / 2 + CameraOffset.X) / Scale &&
                CameraPosition.Y >= (WorldBound.Top + Viewport.Height / 2 + CameraOffset.Y) / Scale &&
                CameraPosition.Y <= (WorldBound.Bottom - Viewport.Height / 2 + CameraOffset.Y) / Scale
                 )
                return true;
            return false;
        }

        
        public void SysmetrizeV(Directions pDir)
            // symetrize the vertical axe of scrollbound
        {
            OffsetK = 1;
            if (pDir == Directions.LEFT)
            {
                OffsetK = -1;
            }
            if (CurrentXDirection != pDir)
            {
                ScrollState = ScrollStates.SYMETRIZE;
                CurrentXDirection = pDir;
                //ScrollOffset = new Vector2(OriginalOffset.X * OffsetK, ScrollOffset.Y);
            }
        }

        protected void UpdateScrollBound()
        {
            //Debug.WriteLine("Updating scroll bound");
            int newX = (int)(CameraPosition.X - ScrollBound.Width / 2 + ScrollBoundOffset.X);
            int newY = (int)(CameraPosition.Y - ScrollBound.Height / 2 + ScrollBoundOffset.Y);
            ScrollBound = new Rectangle(newX, newY, ScrollBound.Width, ScrollBound.Height);
        }

        protected virtual void Move(Vector2 pDeltaMove)
        {
            var oldPosition = CameraPosition;
            CameraPosition += pDeltaMove;
            if (!IsUnderWorld())
            {
                CameraPosition = oldPosition;
            }
            else
            {
                UpdateScrollBound();
            }
        }

        public void SwitchScrollOn()
            // Switch ScrollOn from true to false and vice versa
        {
            ScrollOn = !ScrollOn;
        }

        public virtual void Follow(IActor pSprite, Vector2 pOffset) { }

        public virtual void Update(GameTime gameTime) {}

        public virtual void Set(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: CurrentEffect.Shader);
            //CurrentEffect.Shader.CurrentTechnique.Passes[0].Apply();
        }

        public virtual void Unset(SpriteBatch sb)
        {
            sb.End();
        }
    }
}
