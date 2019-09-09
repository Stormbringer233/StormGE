using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GUI
{
    public class ThemeManager
    {
        public string Name;
        public string MainFont;
        public string BigTitleFont;
        public string MediumTitleFont;
        public string SmalltitleFont;

        public WidgetsDatas Widgets = new WidgetsDatas(); // variable name must be the same as datas section in JSON file
        public WindowsDatas Windows = new WindowsDatas();
    }

    /**
     * ============================= windows datas ===============================
     * */

    public class WindowsDatas // The class don't need to have the same name has json tag.
    {
        public string SheetName;
        public int MainFrameSpaceX;
        public int MainFrameSpaceY;
        public List<WindowsCompleteDatas> WindowsList = new List<WindowsCompleteDatas>(); // same as above

        public WindowsCompleteDatas GetWindowsDatas(string pWindowName)
            // return windows datas according to windows name
        {
            foreach (WindowsCompleteDatas window in WindowsList)
            {
                if (window.Name.ToUpper() == pWindowName.ToUpper())
                {
                    return window;
                }
            }
            return null;
        }
    }

    public class WindowsCompleteDatas
        // This is the main class that store all of single window datas
    {
        public string Name;
       
        public TopBarDatas TopBar = new TopBarDatas();
        public MainFrameDatas MainFrame = new MainFrameDatas();
    }

    public class TopBarDatas
    {
        public class ImageDatas
        {
            public int Xinit;
            public int Yinit;
            public int Width;
            public int Height;
        }
        public class BoundDatas
        {
            public int Xinit;
            public int Yinit;
            public int Width;
            public int Height;
        }
        public ImageDatas Image;
        public BoundDatas Bound;
    }

    public class MainFrameDatas
        // Only basics datas for image are given for the mainframe. The bounding datas are
        // the same as images datas
    {
        public int Xinit;
        public int Yinit;
        public int Width;
        public int Height;
    }

    /**
     * ============================= widget datas ===============================
     * */

    public class WidgetsDatas
    {
        public string SheetName;
        public int Spacing;
        public int Padding;
        public List<ButtonsDatas> ButtonsList = new List<ButtonsDatas>();
        public LabelDatas Labels;

        public ButtonsDatas GetButtonDatas(string buttonName)
        {
            foreach(ButtonsDatas button in ButtonsList)
            {
                if (button.Name.ToUpper() == buttonName.ToUpper())
                {
                    return button;
                }
            }
            return null;
        }


    }

    public class ButtonsDatas
    {
        public struct Boundary
        {
            public int Width;
            public int Height;
        }

        public string WidgetType;
        public string Name;
        public string Type;
        public int TotalAnimations;
        public string Anchor;
        public int Xinit;
        public int Yinit;
        public int Width;
        public int Height;
        public Boundary Bound;
    }

    public class LabelDatas
    {
        public string WidgetType;
        public List<int> NormalColor = new List<int>();
        public List<int> FocusColor = new List<int>();
        public List<int> FreezeColor = new List<int>();
        public List<int> ClickedColor = new List<int>();
        
    }
}
