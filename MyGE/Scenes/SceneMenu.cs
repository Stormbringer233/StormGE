using MyTemplate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using MyGE;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using GUI;
using Microsoft.Xna.Framework.Audio;
using Content.Effects;

namespace Scenes
{
    class SceneMenu : Scene
    {
        Texture2D MenuScreen;
        Player player;
        FadeInTransition FadeIn;
        FadeOutTransition FadeOut;
        FadeInTransition FadeInExit;
        StaticCamera camera;
        SpritesRenderer SpriteRenderer;
        Random rnd;

        public SceneMenu(MainGame pGame) : base(pGame)
        {
            Backgrounds.Add("MENU", new Color(0, 255, 0));
            camera = new StaticCamera(mainGame.GraphicsDevice.Viewport);
            mainGame.GraphicsDevice.Viewport = camera.Viewport;
            FadeIn = new FadeInTransition(OnFadeInEnded);
            FadeOut = new FadeOutTransition(OnFadeOutEnded);
            FadeInExit = new FadeInTransition(Quit, 1);
            rnd = new Random();

            Initialize();
            Debug.WriteLine("MenuGame constructor ended");
        }

        public override void Initialize()
        {
            BackgroundColor = Backgrounds["MENU"];
            render2D = new RenderTarget2D(mainGame.GraphicsDevice, MainGame.SCREEN_WIDTH, MainGame.SCREEN_HEIGHT, false, SurfaceFormat.Color, DepthFormat.Depth24);
            Console.WriteLine("Current background color is " + BackgroundColor);
            camera.SelectEffect("fadeout"); // only for test
            SpriteRenderer = new SpritesRenderer();
            SpriteRenderer.AddEffect("wind", new Wind(MainGame.SCREEN_WIDTH, MainGame.SCREEN_HEIGHT));
            SpriteRenderer.AssignEffect("wind");
            player = new Player(new Vector2(450, 300));
            for (int i = 0; i< 20; i++)
            {
                GameObject o = new GameObject("tree", new Vector2(rnd.Next(0, 750), rnd.Next(0, 550)), Sprite.Anchors.TOPLEFT);
                o.SetScale(0.4f);
                SpriteRenderer.AddSprite(o);
            }
            SpriteRenderer.AddSprite(player);
            base.Initialize();
        }

        public override void Setup()
        {
            oldState = Keyboard.GetState();
            FadeIn.OnTransition = false;
            FadeOut.OnTransition = true;
            Console.WriteLine("Scene Menu Setup ended");
        }

        public override void LoadContent()
        {
            MenuScreen = AssetManager.LoadImage("menuExemple");
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //Console.WriteLine("UnloadContent() of MENU ...");
            base.UnloadContent();
        }

        private void Quit(object sender, EventArgs e)
            // quit the game
        {
            mainGame.Exit();
        }

        public void OnFadeInEnded(object sender, EventArgs e)
        {
            Console.WriteLine("sceneMenu transition IN ended");
            OnSwitchScene();
        }

        public void OnFadeOutEnded(object sender, EventArgs e)
        {
            Console.WriteLine("SceneMenu Transition OUT ended");
        }

        public override void OnSwitchScene()
        {
            OnSwichScene(EventArgs.Empty, "game");
        }

        public override void Update(GameTime gameTime)
        {
            camera.Update(gameTime);
            SpriteRenderer.Update(gameTime);

            KBState = Keyboard.GetState();
            
            if (KBState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape))
                FadeInExit.OnTransition = true;

            MouseWrapper.Update(gameTime);
            //Console.WriteLine("\t\t>> Entering SceneMenu.Update");
            //Console.WriteLine("Yehh, we updating the menu scene !!");
            MltGUI.Update(gameTime);

            if (KBState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                FadeIn.OnTransition = true;

            FadeIn.Update(gameTime);
            FadeOut.Update(gameTime);
            FadeInExit.Update(gameTime);

            base.Update(gameTime);
            //Console.WriteLine("\t\t<< Finish SceneMenu.Update");
            oldState = KBState;
        }

        public override void Draw(GameTime gameTime)
        {
            
            var sb = mainGame.spriteBatch;

            //mainGame.GraphicsDevice.SetRenderTarget(render2D);
            mainGame.GraphicsDevice.Clear(BackgroundColor);

            camera.Set(sb); // begin spriteBatch into camera to be applied to all sprites into the scene

            sb.Draw(MenuScreen, new Vector2(ScreenWidth / 2, ScreenHeight / 2), null, Color.White, 0,
                    new Vector2(MenuScreen.Width / 2, MenuScreen.Height / 2), 1f, SpriteEffects.None, 0);

            SpriteRenderer.Draw(sb, gameTime);

            ////MltGUI.Draw(sb, gameTime);

            ////FadeIn.Draw(sb, gameTime);
            ////FadeOut.Draw(sb, gameTime);
            ////FadeInExit.Draw(sb, gameTime);
            camera.Unset(sb);

            //mainGame.GraphicsDevice.SetRenderTarget(null);
            if (MainGame.ShowDebug)
            {
                sb.Begin();
                DrawDebug(sb, gameTime);
            }
            //sb.Draw(render2D, Vector2.Zero, Color.White);
            sb.End();

            //base.Draw(gameTime);
        }

        private void DrawDebug(SpriteBatch sb, GameTime gameTime)
        {
            Color textCol = Color.Red;
            sb.DrawString(font, "SceneMenu of  - To be Implement !", new Vector2(50, ScreenHeight - 20), textCol, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            sb.DrawString(font, "Work in progress", new Vector2(600, ScreenHeight - 20), textCol, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            sb.DrawString(font, "Mouse position : " + MouseWrapper.Position, new Vector2(0, 0), textCol);
        }
    }
}
