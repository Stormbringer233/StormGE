using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using Newtonsoft.Json;
using MyGE;

namespace Scenes
{
    public class LevelManager
    {
        private MainGame mainGame;
        private Texture2D Background;

        public LevelManager(MainGame pGame)
        {
            mainGame = pGame;
        }

        public void Load(int pNumber)
        {
            string fullPath = MainConfig.ConfigDatas.LevelsFolder + "Lvl" + pNumber.ToString() + ".Json";
            string LevelDatas = File.ReadAllText(fullPath);

            //Console.WriteLine("Level loaded");
        }

        public void Move(Vector2 pDelta)
            // Move the background Level.
        {
            //Position -= pDelta;
        }

        public void Update(GameTime gameTime)
        {
            //foreach (Dude dude in BadGuys)
            //{
            //    dude.Update(gameTime);
            //}
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
        }
    }
}
