using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public static class MainConfig
    {
        public static ConfigDatasReader ConfigDatas; // the main configuration file

        public static void Initialize()
        {
            // define the main configuration of the window
            string fromFile = File.ReadAllText("Configs.json");
            ConfigDatas = JsonConvert.DeserializeObject<ConfigDatasReader>(fromFile);
        }
    }

}
