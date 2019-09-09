using Content.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class GameObject
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Friction { get; set; }
        public int Mass { get; set; }

        protected Sprite Sprite;
        // *************** PRIVATE ***************
        bool gravity;


        public GameObject()
        {
            Sprite = null;
            Initialize();
        }

        public GameObject(string pName, Vector2 pPosition, Sprite.Anchors pAnchor)
        {
            Sprite = new Sprite(pName, pPosition, pAnchor);
            Initialize();
        }

        public GameObject(SpriteDatas pDatas, Vector2 pPosition)
        {
            Sprite = new Sprite(pDatas, pPosition);

            Initialize();
        }

        private void Initialize()
        {
            Velocity = Vector2.Zero;
            Friction = Vector2.Zero;
            Mass = 0;
            gravity = false;
        }

        public void AddHitBox(string pAnimationName, Rectangle pHitbox)
        {
            Sprite.AddHitbox(pAnimationName, pHitbox);
        }

        public List<Rectangle> GetHitBox()
        {
            return Sprite.GetHitbox();
        }

        public void SetGravityOn()
        {
            gravity = true;
        }

        public void SetGravityOff()
        {
            gravity = false;
        }

        public void SetScale(float pScale)
        {
            Sprite.Scale = pScale;
        }

        public virtual void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            Sprite.Draw(sb, gameTime);
        }

    }
}
