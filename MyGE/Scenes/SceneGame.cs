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

namespace Scenes
{
    class SceneGame : Scene
    {
        Texture2D MenuScreen;
        SceneTransition FadeOut;
        SceneTransition FadeIn;

        public SceneGame(MainGame pGame) : base(pGame)
        {
            MenuScreen = AssetManager.LoadImage("menuExemple");
            Backgrounds.Add("GAME", new Color(0, 200, 150));
            FadeOut = new FadeOutTransition(OnFadeOutEnded);
            FadeIn = new FadeInTransition(OnFadeInEnded);
            Initialize();
            Debug.WriteLine("Game constructor ended");
        }

        public override void Initialize()
        {
            BackgroundColor = Backgrounds["GAME"];
            Console.WriteLine("Current background color is " + BackgroundColor);
            base.Initialize();
        }

        public override void Setup()
        {
            // Reinitialize the old keyboard state to prevent a fast switch to the previous Scene in case of
            // the key to switch is the same for each scene
            oldState = Keyboard.GetState();
            FadeIn.OnTransition = false;
            FadeOut.OnTransition = true;
            Console.WriteLine("Scene game Setup ended");
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //Console.WriteLine("UnloadContent() of MENU ...");
            base.UnloadContent();
        }

        public void OnFadeInEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Scene game transition IN ended");
            OnSwitchScene();
        }

        public void OnFadeOutEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Scene game transition OUT ended");
        }

        public override void OnSwitchScene()
            // switch to game screen
        {
            OnSwichScene(EventArgs.Empty, "menu");
        }

        private void Quit()
            // quit the game
        {
            mainGame.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            KBState = Keyboard.GetState();
            MouseWrapper.Update(gameTime);
            //Console.WriteLine("\t\t>> Entering SceneMenu.Update");
            //Console.WriteLine("Yehh, we updating the menu scene !!");
            MltGUI.Update(gameTime);

            if (KBState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                FadeIn.OnTransition = true;

            if (KBState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape))
                FadeIn.OnTransition = true;

            oldState = KBState;

            FadeIn.Update(gameTime);
            FadeOut.Update(gameTime);

            base.Update(gameTime);
            //Console.WriteLine("\t\t<< Finish SceneMenu.Update");

        }

        public override void Draw(GameTime gameTime)
        {
            mainGame.GraphicsDevice.Clear(BackgroundColor);
            var sb = mainGame.spriteBatch;

            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap);

            //sb.Draw(MenuScreen, new Vector2(ScreenWidth / 2, ScreenHeight / 2), null, Color.White, 0,
            //        new Vector2(MenuScreen.Width / 2, MenuScreen.Height / 2), 1f, SpriteEffects.None, 0);

            MltGUI.Draw(sb, gameTime);

            //FadeIn.Draw(sb, gameTime);
            //FadeOut.Draw(sb, gameTime);

            if (MainGame.ShowDebug)
            {
                sb.DrawString(font, "SceneGame of  - To be Implement !", new Vector2(50, ScreenHeight - 20), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                sb.DrawString(font, "Work in progress", new Vector2(600, ScreenHeight - 20), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                sb.DrawString(font, "Mouse position : " + MouseWrapper.Position, new Vector2(0,0), Color.White);
            }

            sb.End();

            base.Draw(gameTime);
        }
    }
}
