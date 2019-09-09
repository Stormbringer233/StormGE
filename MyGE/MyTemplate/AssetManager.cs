using MyGE;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MyTemplate
{
    public static class AssetManager
    // Classe singleton
    // Gestion des ressources du jeu (images, sons, musiques, fonts, ...)
    {
        public enum Usages { BACKGROUND, ASSET, FONT, SOUND}
        public static Usages Usage;
        public static SpriteFont MainFont { get; set; }

        //AssetManager() { }
        //static AssetManager uniqueInstance;
        static Dictionary<string, Effect> listEffects;
        static Dictionary<string, Texture2D> listImages; // stocke les images en mémoire sous la forme {nom fichier, image}
        static Dictionary<string, SpriteFont> listFonts; // stock the list of all used fonts
        static Dictionary<string, Song> listSongs;
        static Dictionary<string, SoundEffect> listSounds;
        static ContentManager content;
        static GraphicsDevice device;
        static MainGame mainGame;
        //public static AssetManager GetInstance()
        //    // Retourne 
        //{
        //        if (uniqueInstance == null)
        //            uniqueInstance = new AssetManager();
        //        return uniqueInstance;
        //}

        public static void Initialize(MainGame pGame)
        {
            mainGame = pGame;
            content = pGame.Content;
            device = pGame.GraphicsDevice;

            listEffects = new Dictionary<string, Effect>();
            listImages = new Dictionary<string, Texture2D>();
            listFonts = new Dictionary<string, SpriteFont>();
            listSounds = new Dictionary<string, SoundEffect>();
            listSongs = new Dictionary<string, Song>(); // To review. It is useles to load song (to much size in memory)
            Console.WriteLine("Assets.Initialize() terminé");
        }

        public static Effect LoadEffect()
        {
            return LoadEffect("DefaultEffect");
        }

        public static Effect LoadEffect(string pEffectName)
        {
            string name = pEffectName;
            if (listEffects.ContainsKey(name))
            {
                return listEffects[name];
            }
            string path = MainConfig.ConfigDatas.AssetsFolders.Effects;
            Effect effect = content.Load<Effect>(path + name);
            listEffects.Add(name, effect);
            return effect;
        }

        public static SpriteFont LoadFont()
        {
            MainFont = content.Load<SpriteFont>("DefaultFont");
            return MainFont;
        }

        public static Texture2D LoadImage(string pImageName)
        {
            if (listImages.ContainsKey(pImageName))
            {
                //Texture2D newTex = new Texture2D(device, listImages[pImageName].Width, listImages[pImageName].Height);
                //newTex = listImages[pImageName];
                //return newTex;
                return listImages[pImageName];
            }
            // sinon, on ajoute l'image au dictionnaire
            string path = MainConfig.ConfigDatas.AssetsFolders.Graphics;
            path += pImageName;
            var tempImage = content.Load<Texture2D>(path);
            listImages.Add(pImageName, tempImage);
            return tempImage;

        }

        public static SpriteFont LoadFont(string pFontName)
            // Load and return the require font.
            // Be carefull the spriteFont is add to ContentMAnager
        {
            if (listFonts.ContainsKey(pFontName))
                return listFonts[pFontName];
            var tempFont = content.Load<SpriteFont>(pFontName);
            listFonts.Add(pFontName, tempFont);
            return tempFont;
        }
    }


}
