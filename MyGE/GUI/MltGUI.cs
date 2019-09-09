using System;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MyTemplate;
using MyGE;
using Microsoft.Xna.Framework.Graphics;

namespace GUI
{
    /**
     * MltGUI is a static class to manage all of the GUI behaviors in games
     * This class is create to load the theme of the GUI, saving all of widgets to updating and drawing them in a pack
     * 
     * M. Le Thiec
     * creation date : 20/10/2018
     * V : 0.00
     * 
     * 
     * TODO :
     *      - Add a structure to store widgets in composite style
     * */

    public static class MltGUI
    {
        public static ThemeManager Theme { get; private set; }
        public static float Scale;

        static List<Widget> WidgetList;

        public static void Initialize(string pThemeName)
        {
            Console.WriteLine("Initializing GUI ...");
            WidgetList = new List<Widget>();
            string fullPath = MainConfig.ConfigDatas.GUIFolder + pThemeName;
            string themeJSONFile = File.ReadAllText(fullPath + ".json");
            Theme = JsonConvert.DeserializeObject<ThemeManager>(themeJSONFile);
            Scale = MainConfig.ConfigDatas.Scale;
            Console.WriteLine("GUI initialization ended");
        }

        public static void Add(Widget pWidget)
        {
            WidgetList.Add(pWidget);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Widget widget in WidgetList)
            {
                widget.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch sb, GameTime gameTime)
        {
            //mainGame.spriteBatch.Begin();
            sb.End();
            sb.Begin();
            foreach (Widget widget in WidgetList)
            {
                widget.Draw(sb, gameTime);
            }

        }

    }
}
