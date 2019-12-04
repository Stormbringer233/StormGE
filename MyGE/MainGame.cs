//using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using Newtonsoft.Json;
using Scenes;
using System;
using System.IO;

namespace MyGE
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        public static bool ShowDebug;
        public static int SCREEN_WIDTH;
        public static int SCREEN_HEIGHT;
        Effect FadeOutEffect;

        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        //public AssetManager Assets;
        public string NextScene = "";

        // static constructor
        static MainGame()
        {
            ShowDebug = true;
            MouseWrapper.Initialize();
            ScenesManager.Initialize();
            MainConfig.Initialize();
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            graphics.PreferredBackBufferWidth = MainConfig.ConfigDatas.WindowWidth;
            graphics.PreferredBackBufferHeight = MainConfig.ConfigDatas.WindowHeight;
            SCREEN_WIDTH = MainConfig.ConfigDatas.WindowWidth;
            SCREEN_HEIGHT = MainConfig.ConfigDatas.WindowHeight;
            Window.Title = MainConfig.ConfigDatas.GameName + " - " + " V : " + MainConfig.ConfigDatas.Version + " Build Date : " + MainConfig.ConfigDatas.BuildDate;
            // mouse is visible. For developpement time
            IsMouseVisible = true;
            Console.WriteLine("mainGame Constructor ended");
        }

        protected override void Initialize()
        {
            Console.WriteLine("mainGame Initialize() Begin");
            // Initialize asset manager
            AssetManager.Initialize(this);

            // Initialize GUI -- ATTENTION : Before scene initialization
            //MltGUI.Initialize("BuildTheme");

            // Initialize gameState
            ScenesManager.Add("menu", new SceneMenu(this));
            ScenesManager.Add("game", new SceneGame(this));
            ScenesManager.SwitchTo("menu");

            Console.WriteLine("mainGame Initialize() ended");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Primitive.Initialize(spriteBatch, GraphicsDevice);
            FadeOutEffect = Content.Load<Effect>("FadeOut");
            Console.WriteLine("mainGame Loadcontent() ended");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // TODO: Add your update logic here
            ScenesManager.Update(gameTime);

            //MouseWrapper.UpdateOldStates();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Give the drawing responsability to the game manager
            ScenesManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
