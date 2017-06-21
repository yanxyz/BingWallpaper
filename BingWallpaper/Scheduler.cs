using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BingWallpaper
{
    class Scheduler
    {
        public static void Create()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $@"/create /tn BingWallpaperTask /tr ""{App.ExecutablePath} -s"" /sc daily /st 11:00 /f",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Debug.WriteLine(startInfo.FileName + " " + startInfo.Arguments);
            var proc = Process.Start(startInfo);
            proc.WaitForExit(10000);
            if (proc.ExitCode > 0)
                throw new Exception("创建计划任务失败，点击“修改”按钮手动创建。");
        }

        public static void Open()
        {
            Process.Start("Taskschd.msc");
        }

        public static void Delete()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $@"/delete /tn BingWallpaperTask /f",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(startInfo);
        }
    }
}
