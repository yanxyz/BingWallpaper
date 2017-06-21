using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using fastJSON;

namespace BingWallpaper
{
    public class ImageData
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }
        public string Copyright { get; set; }
        public string CopyrightLink { get; set; }
        public string Story { get; set; }
        // not exported
        public string UrlBase;
        public string Path;
        public string DateString;
        public string Bing;

        public Dictionary<string, string> Export()
        {
            var dict = new Dictionary<string, string>
            {
                ["Title"] = this.Title,
                ["Date"] = this.Date,
                ["Url"] = this.Url,
                ["Copyright"] = this.Copyright,
                //["CopyrightLink"] = this.CopyrightLink,
                ["Story"] = this.Story
            };
            return dict;
        }
    }

    public class Wallpaper
    {
        public const string Bing = "https://www.bing.com";
        private static string CacheDir = Path.Combine(App.StartupPath, "Cache");
        public static string TodayString = DateTime.Today.ToString("yyyyMMdd");
        public static string TodayImage = Path.Combine(CacheDir, TodayString + ".jpg");

        public static async Task<ImageData> Download(DateTime? selectedDate)
        {
            ImageData img;
            var date = selectedDate ?? DateTime.Today;
            var dateString = date.ToString("yyyyMMdd");

            var imageFile = Path.Combine(CacheDir, dateString + ".jpg");
            var jsonFile = Path.Combine(CacheDir, dateString + ".json");

            if (File.Exists(imageFile) && File.Exists(jsonFile))
            {
                using (StreamReader sr = new StreamReader(jsonFile))
                {
                    img = JSON.ToObject<ImageData>(sr.ReadToEnd());
                    System.Diagnostics.Debug.WriteLine(img.Date);
                    if (img.DateString == dateString && img.Path == imageFile)
                        return img;
                }
            }

            var jsonParams = new JSONParameters
            {
                UseEscapedUnicode = false
            };

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                // get image info
                var idx = (DateTime.Today - date).Days;
                var text = await client.DownloadStringTaskAsync($"http://www.bing.com/HPImageArchive.aspx?format=js&idx={idx}&n=1");
                var data = JSON.ToDynamic(text);
                var img0 = data.images[0];
                string copyrightText = img0.copyright;
                var i = copyrightText.IndexOf(" (");
                var title = copyrightText.Substring(0, i);
                var copyright = copyrightText.Substring(i + 2, copyrightText.Length - i - 3);
                img = new ImageData
                {
                    Title = title,
                    Url = img0.url,
                    UrlBase = img0.urlbase,
                    Copyright = copyright,
                    CopyrightLink = img0.copyrightlink,
                    Date = date.ToString("yyyy/MM/dd"),
                    DateString = dateString,
                    Path = imageFile,
                    Bing = Bing
                };

                // get story
                if (img.CopyrightLink.IndexOf("mkt=zh-cn") > -1)
                {
                    text = await client.DownloadStringTaskAsync($"http://cn.bing.com/cnhp/coverstory/?d={dateString}");
                    data = JSON.ToDynamic(text);
                    img.Story = data.para1;
                    img.Bing = "https://cn.bing.com";
                }
            }

            // download image
            if (!Directory.Exists(CacheDir)) Directory.CreateDirectory(CacheDir);
            await DownloadPicture(img);

            // write json
            using (StreamWriter sw = new StreamWriter(jsonFile))
            {
                await sw.WriteAsync(JSON.ToJSON(img, jsonParams));
            }

            return img;
        }

        private static async Task DownloadPicture(ImageData img)
        {
            var url = img.Bing + img.Url;

            var w = SystemParameters.PrimaryScreenWidth;
            var h = SystemParameters.PrimaryScreenHeight;
            // prefer 1920x1080
            if (w < 1366)
            {
                var testUrl = img.Bing + img.UrlBase + $"_{w}x{h}.jpg";
                var request = WebRequest.Create(testUrl);
                request.Method = "HEAD";
                var response = await request.GetResponseAsync();
                if ((response as HttpWebResponse).StatusCode == HttpStatusCode.OK)
                    url = testUrl;
            }

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(url, img.Path);
            }
        }

        public static void ClearCache()
        {
            var files = Directory.EnumerateFiles(CacheDir);
            foreach (var currentFile in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(currentFile);
                if (fileName != TodayString) File.Delete(currentFile);
            }
        }

        internal sealed class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        }

        public static void SetBackground(string picturePath)
        {
            // 复制到“我的图片”，避免程序移动之后壁纸失效
            var wallpaperPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "BingWallpaper.jpg");
            File.Copy(picturePath, wallpaperPath, true);

            const int SetDesktopBackground = 20;
            const int UpdateIniFile = 1;
            const int SendWindowsIniChange = 2;
            NativeMethods.SystemParametersInfo(SetDesktopBackground, 0, wallpaperPath, UpdateIniFile | SendWindowsIniChange);
        }

        public static void Save(ImageData img)
        {
            var dir = ConfigManager.GetSaveDirectory();
            if (String.IsNullOrEmpty(dir))
                throw new Exception("收藏夹不存在，打开'设置对话框'添加收藏夹。");

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.Copy(img.Path, Path.Combine(dir, img.DateString + ".jpg"), true);

            // 将图片信息保存到 .json 文件
            using (StreamWriter sw = new StreamWriter(Path.Combine(dir, img.DateString + ".json")))
            {
                var jsonParams = new JSONParameters
                {
                    UseExtensions = false,
                    UseEscapedUnicode = false,
                    SerializeToLowerCaseNames = true
                };
                sw.Write(JSON.ToNiceJSON(img.Export(), jsonParams));
            }
        }
    }
}
