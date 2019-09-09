using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGE;

namespace MyTemplate
{
    public class Sprite : IActor
    {
        public enum Anchors { CENTER, TOPLEFT}
        public Anchors Anchor;
        public Animation CurrentAnimation { get; set; }
        public Vector2 Position { get; set;}
        public float Scale; // scale of sprites
        public float Angle { get; set; }

        public Vector2 Origin { get; set; }
        protected Texture2D texture;
        protected Dictionary<string, Animation> SpriteAnimations;
        protected Vector2 InitialPosition { get; set; }
        protected SpriteEffects Effect { get; set; }
        protected Color TextureColor { get; set; }

        public Sprite(Vector2 pPosition)
            // Create a character without any image attach.
            // must be create in derived class
        {
            texture = null; // Be carefull !!!!
            Position = pPosition;
            InitialPosition = pPosition;
            // default values at creation of sprite. Can be change later
            Origin = Vector2.Zero;
            Effect = SpriteEffects.None;
            Scale = 1;
            TextureColor = Color.White;
        }


        public Sprite(string pImageName, Vector2 pPosition, Anchors pAnchor)
            // Build sprite without attach animation and without json file to read
            // pImageName : name of the image without extension
        {
            texture = AssetManager.LoadImage(pImageName);
            // search for any animation file with the same name
            // attach font to the sprite in debug fonctions
            Position = pPosition;
            InitialPosition = pPosition;
            // default values at creation of sprite. Can be change later
            switch (pAnchor)
            {
                case Anchors.CENTER:
                    Origin = new Vector2(texture.Width / 2, texture.Height / 2);
                    break;
                case Anchors.TOPLEFT:
                    Origin = new Vector2(0,0);
                    break;
                default:
                    Origin = new Vector2(texture.Width / 2, texture.Height / 2);
                    break;
            }
            // By default the hit box is the plain image
            Scale = 1;
            TextureColor = Color.White;
            Effect = SpriteEffects.None;

            SpriteAnimations = new Dictionary<string, Animation>();
            SpriteAnimations.Add("NONE", new Animation(texture.Width, texture.Height));
            // default values at creation of sprite. Can be change later
            CurrentAnimation = SpriteAnimations["NONE"];
        }

        public Sprite(SpriteDatas pDatas, Vector2 pPosition)
        // Build a sprite object with animation datas.
        // In this case, the datas are load in the main objet and only pass to the sprite.
        // So, main object must have some lines like that :

        // string fullpath = mainGame.ConfigDatas.CharactersFolder + CharacterJSONFile + ".json";
        // string animJSONFile = File.ReadAllText(fullpath);
        // SpriteSheetManager Datas = JsonConvert.DeserializeObject<SpriteSheetManager>(animJSONFile);
        {

            texture = AssetManager.LoadImage(pDatas.SpriteSheet);
            Position = pPosition;
            InitialPosition = pPosition;
            Scale = 1;
            Effect = SpriteEffects.None;
            TextureColor = new Color(pDatas.TextureColor[0], pDatas.TextureColor[1], pDatas.TextureColor[2], pDatas.TextureColor[3]);
            // build animations frome datas
            SpriteAnimations = new Dictionary<string, Animation>();
            if (pDatas.AnimDatas.Count > 0)
            {
                foreach (AnimationDatas datas in pDatas.AnimDatas)
                {
                    SpriteAnimations.Add(datas.Name, new Animation(datas, pDatas.FrameWidth, pDatas.FrameHeight));
                    // Reallocate origin corresponding to animation
                    Origin = SpriteAnimations[datas.Name].Origin;
                }
            }
            else
            {
                // build a single quad that represent the entire image
                SpriteAnimations.Add("IDLE", new Animation(pDatas.FrameWidth, pDatas.FrameHeight));
                // default values at creation of sprite. Can be change later
                Origin = new Vector2(texture.Width / 2, texture.Height / 2);
                CurrentAnimation = SpriteAnimations["IDLE"];
            }
        }

        public void AddAnimation(string pName, int[] pFrameOrder, float pFrameRate,float pDelayAtEnd, int pLoop, Vector2 pFrameDims, bool pFlipH, bool pFlipV)
        {
            // Add a new animation into the set of animations
            /// <param name="pName">the name of the animation</param>
            /// <param name="pFrameOrder">int array that give the order of frames to play animation</param>
            /// <param name="pFrameRate">the frame animation duration</param>
            /// <param name="pLoop">is animation loop indefinitely = -1 or a number of loop > 0</param>
            /// <param name="pFrameDim">the width and the height of a frame (all frames must have the sames dimensions)</param>
            /// <param name="pFlipH">is animation can flip horizontaly</param>
            /// <param name="pFlipV">is animation can flip verticaly</param>
            /// 
            pName = pName.ToUpper();
            if (SpriteAnimations.ContainsKey(pName))
                return; // an animation can not be add twice
            SpriteAnimations.Add(pName,
                new Animation(pFrameOrder, pFrameRate, pDelayAtEnd, pLoop, new Vector2(texture.Width, texture.Height), pFrameDims, pFlipH, pFlipV)
                );
            if (CurrentAnimation == null)
                CurrentAnimation = SpriteAnimations[pName];
        }

        public void InitializePosition()
        {
            InitializePosition(Position);
        }

        public void InitializePosition(Vector2 pNewPosition)
        {
            Position = pNewPosition;
            InitialPosition = pNewPosition;
        }

        public void SetAnimation(string pNewAnimation)
        {
            if (SpriteAnimations.ContainsKey(pNewAnimation.ToUpper()))
            {
                CurrentAnimation = SpriteAnimations[pNewAnimation.ToUpper()];
                //CurrentAnimation.RestartLoop = true;
            }
        }

        public void PlayAnimation()
        {
            CurrentAnimation.Play();
        }

        public void PlayAnimation(string pNewAnimation)
        {
            SetAnimation(pNewAnimation);
            PlayAnimation();
        }

        public void AddHitbox(string pAnimationName, Rectangle pHitbox)
        {
            pAnimationName = pAnimationName.ToUpper();
            if (SpriteAnimations.ContainsKey(pAnimationName))
                SpriteAnimations[pAnimationName].AddHitBox(pHitbox);
        }

        public List<Rectangle> GetHitbox()
        {
            return CurrentAnimation.ListHitbox;
        }

        public bool AnimationEnded()
        {
            if (!CurrentAnimation.OnAnimation)
            {
                CurrentAnimation.RestartLoop = true;
                return true;
            }
            return false;
        }

        public void RestoreBasePosition()
        {
            Position = InitialPosition;
        }

        public void Rotate(float pdeltaAngle)
        {
            Angle += pdeltaAngle % MathHelper.ToRadians(360);
        }

        public void RotateClock()
            // Rotate 90° clockwise
        {
            Angle += MathHelper.ToRadians(90) % MathHelper.ToRadians(360);
        }

        public void RotateAntiClock()
        {
            Angle -= MathHelper.ToRadians(90) % MathHelper.ToRadians(360);
        }

        public virtual void Move(int pDX, int pDY)
        {
            Move(new Vector2(pDX, pDY));
        }

        public virtual void Move(Vector2 pDeltaPosition)
        {
            Position += pDeltaPosition;
        }

        public virtual void MoveAt(Vector2 pDest)
        {
            Position = pDest;
        }

        public void SetScale(float pNewScale)
        {
            Scale = pNewScale;
        }

        public float GetScale()
        {
            return Scale;
        }

        protected virtual void UpdateOrigin()
        {
            if (Effect == SpriteEffects.FlipHorizontally)
            {
                // In case of flip, and origin not in center of frame, we need to update origin correctly.
                var curOrigin = CurrentAnimation.Origin;
                Origin = new Vector2(CurrentAnimation.CurrentQuad.Width - curOrigin.X, curOrigin.Y);
            }
            else
                Origin = CurrentAnimation.Origin;
        }

        public virtual void Update(GameTime gameTime)
        {
            CurrentAnimation.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
            // Sort of Draw methode with spriteBatch as parameter
        {
            sb.Draw(texture, Position, CurrentAnimation.CurrentQuad, TextureColor, Angle, Origin, Scale, Effect, 0);
        }
    }
}
