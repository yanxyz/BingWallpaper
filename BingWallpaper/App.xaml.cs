using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;

namespace BingWallpaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static Assembly myAssembly = Assembly.GetEntryAssembly();
        public static string ExecutablePath = myAssembly.Location;
        public static string StartupPath = Path.GetDirectoryName(ExecutablePath);
        public static string SemVer
        {
            get {
                var ver = myAssembly.GetName().Version;
                return ver.Major + "." + ver.Minor;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args[0] == "-s")
            {
                if (File.Exists(Wallpaper.TodayImage))
                {
                    this.Shutdown();
                }
            }
        }
    }
}
