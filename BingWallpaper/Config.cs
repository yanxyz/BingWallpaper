using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using fastJSON;

namespace BingWallpaper
{
    public class ConfigData
    {
        public string SaveDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "BingWallpaper");
    }

    public class ConfigManager
    {
        private static string configFile = Path.ChangeExtension(App.ExecutablePath, ".json");
        public static ConfigData Config = new ConfigData();

        public static void LoadConfig()
        {
            if (File.Exists(configFile))
            {
                using (StreamReader sr = new StreamReader(configFile))
                {
                    Config = JSON.ToObject<ConfigData>(sr.ReadToEnd());
                }
            }
        }

        public static void SaveConfig()
        {
            using (StreamWriter sw = new StreamWriter(configFile))
            {
                sw.Write(JSON.ToJSON(Config));
            }
        }

        public static string GetSaveDirectory()
        {
            var dir = Config.SaveDirectory;
            if (dir.Length == 0) return dir; 
            return Path.Combine(App.StartupPath, Environment.ExpandEnvironmentVariables(dir));
        }
    }
}
