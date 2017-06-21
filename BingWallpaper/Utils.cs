using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BingWallpaper
{
    public class Utils
    {
        public static void OpenDir(string dir)
        {
            var tc = Environment.GetEnvironmentVariable("COMMANDER_EXE");

            if (String.IsNullOrEmpty(tc))
            {
                Process.Start(dir);
            }
            else
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = tc,
                    Arguments = $"/O /T /S /L=\"{dir}\""
                };
                Process.Start(startInfo);
            }
        }
    }
}
