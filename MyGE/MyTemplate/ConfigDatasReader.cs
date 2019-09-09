using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class ConfigDatasReader
    {
        public string GameName;
        public string Version;
        public string BuildDate;
        public int BlowRange;
        public float Scale;
        public int WindowWidth;
        public int WindowHeight;
        public bool ShowDebug;
        public string GameObjectsFolder;
        public string LevelsFolder;
        public string GUIFolder;
        public FontsDatas Fonts;
        public AssetsFolders AssetsFolders;
        public PadsDatas Pads;
    }

    public class FontsDatas
    {
        public string DefaultFont; // the arial basic font for everything we needs
        public string MainFont;
        public string BigFont;
        public string MediumFont;
        public string SmallFont;
    }

    public class PadsDatas
    {
        public string Player1;
        public string Player2;
    }

    public class AssetsFolders
    {
        public string Graphics;
        public string Sounds;
        public string Fonts;
        public string Effects;
    }
}
