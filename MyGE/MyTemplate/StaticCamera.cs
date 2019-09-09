using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyTemplate
{
    /// <summary>
    /// Implement a static and basic camera. It not able to move in any direction.
    /// These camera can only :
    ///     - rotate around a center
    ///     - modify scale
    /// By default, the center point is set to the center of the viewport
    /// </summary>
    public class StaticCamera : Camera2D
    {
        public StaticCamera(Viewport pViewport) : base(pViewport)
        {
            Initialize();
        }

        public StaticCamera(Viewport pViewport, Rectangle pRegion) : base(pViewport, pRegion)
        {
            Initialize();
        }

        private void Initialize()
        {
            CameraPosition = new Vector2(-Viewport.Width / 2, -Viewport.Height / 2);
            WorldBound = Viewport.Bounds;
            Scale = 1;
            AngleZ = 0;
            RotateCenter = Viewport.Bounds.Center;
            CameraPosition = new Vector2(RotateCenter.X, RotateCenter.Y);
        }

        public override void Update(GameTime gameTime)
        {
            CurrentEffect.Update(gameTime);
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
