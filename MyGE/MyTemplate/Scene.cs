using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenes;

namespace MyTemplate
{
    public abstract class Scene
    {
        // Protected
        protected MainGame mainGame;
        protected int ScreenWidth;
        protected int ScreenHeight;
        protected float ScreenRatio;
        protected SpriteFont font;
        protected SpriteFont debugFont;
        protected float Scale { get; }
        protected Color BackgroundColor;
        protected Dictionary<string, Color> Backgrounds;
        protected bool ShowDebug;
        protected KeyboardState oldState;
        protected KeyboardState KBState;
        protected Timer FadeSwitchTimer;
        protected RenderTarget2D render2D;

        // Public
        public delegate void SwitchSceneEventHandler(object sender, EventArgs e, string pToScene);
        public event SwitchSceneEventHandler SwitchScene;
        public string SceneName { get; set; }
        //protected KeyboardState OldState;
        //protected KeyboardState CurrentState;


        public Scene(MainGame pGame)
        {
            //Console.WriteLine("Entering Constructor of Abstract Scene");
            mainGame = pGame;
            ScreenWidth = pGame.GraphicsDevice.Viewport.Width;
            ScreenHeight = pGame.GraphicsDevice.Viewport.Height;
            ScreenRatio = pGame.GraphicsDevice.Viewport.AspectRatio;
            Scale = MainConfig.ConfigDatas.Scale;
            ShowDebug = MainGame.ShowDebug;
            Backgrounds = new Dictionary<string, Color>
            {
                { "DEFAULT", new Color(0, 0, 0)},
                { "SAVED", new Color(0,0,0) }
            };
            oldState = Keyboard.GetState();
            FadeSwitchTimer = new Timer(0.5);
            //mainGame.graphics.ToggleFullScreen();
            //Console.WriteLine("Viewport width, height = " + ScreenWidth + " , " + ScreenHeight + "\tAspect ratio = " + ScreenRatio);
        }

        public virtual void Initialize() { }

        public abstract void Setup();

        public virtual void Restore() { }

        public virtual void LoadContent()
        {
            //Console.WriteLine("Entering Scene.LoadContent()");
            font = AssetManager.LoadFont(MainConfig.ConfigDatas.Fonts.DefaultFont);
            debugFont = AssetManager.LoadFont(MainConfig.ConfigDatas.Fonts.DefaultFont);
        }

        public virtual void UnloadContent() { }

        protected virtual void UpdatePlayerBehavior()
        {
        }

        public virtual void AddPlayer(Player pPlayer) { }

        public virtual void OnSwitchScene() { }

        public virtual void OnSwichScene(EventArgs e, string pToScene)
        {
            SwitchScene?.Invoke(this, e, pToScene);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {
            var sb = mainGame.spriteBatch;
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap);

            if (ShowDebug)
            {
                sb.DrawString(font, "Current scene = " + SceneName, new Vector2(ScreenWidth - 250, 0), Color.Red);
                //sb.DrawString(font, "player One moveBehavior : " + mainGame.Players[PlayerIndex.One].MoveBehavior.ToString(), new Vector2(0, 800), Color.White);
                //sb.DrawString(font, "player Two moveBehavior : " + mainGame.Players[PlayerIndex.Two].MoveBehavior.ToString(), new Vector2(0, 840), Color.White);
            }
            sb.End();
        }
    }
}
