using Newtonsoft.Json;
using MyGE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    /**
     * Base class for initialize GUI with right theme
     * GUIBase initialize a container to store all of widgets in it
     * M. Le Thiec
     * 07/10/2018
     * 
     * V : 0.00
     * */

    sealed public class GUIBase : IComposite
    {
        private List<Widget> GUiList;
        public MainGame mainGame;
        private ThemeManager Theme;

        public GUIBase(MainGame pGame, string pThemeName)
        {
            mainGame = pGame;
            string themeJSONFile = File.ReadAllText(pThemeName + ".json");
            ThemeManager Theme = JsonConvert.DeserializeObject<ThemeManager>(themeJSONFile);
            GUiList = new List<Widget>();
        }

        public void Add(Widget pElement)
        {
            if (!GUiList.Contains(pElement))
                GUiList.Add(pElement);
        }

        public Widget GetChild(int pIndex)
        {
            if (pIndex < GUiList.Count)
                return GUiList[pIndex];
            return null;
        }

        public void Remove(Widget pElement)
        {
            if (GUiList.Contains(pElement))
            {
                GUiList.Remove(pElement);
            }
        }
    }
}
