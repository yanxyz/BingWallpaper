using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace BingWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageData img;

        public MainWindow()
        {
            InitializeComponent();
            cld.DisplayDateStart = DateTime.Today.AddDays(-7);
            cld.DisplayDateEnd = DateTime.Now;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadWallpaper();
        }

        private async Task LoadWallpaper(DateTime? date = null)
        {
            WallpaperInfo.DataContext = new ImageData
            {
                Title = "加载中..."
            };

            try
            {
                img = await Wallpaper.Download(date);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Debug.WriteLine(img.Path);
            SetImageSource();
            WallpaperInfo.DataContext = img;
        }

        private void SetImageSource()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(img.Path);
            image.EndInit();
            WallpaperImage.Source = image;
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            Wallpaper.SetBackground(img.Path);
        }

        private void Bing_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start(img.CopyrightLink);
            Process.Start(Wallpaper.Bing);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsWindow();
            dialog.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/yanxyz/BingWallpaper/");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Wallpaper.Save(img);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dir = ConfigManager.GetSaveDirectory();
            Utils.OpenDir(dir);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfigManager.SaveConfig();
            Wallpaper.ClearCache();
        }

        private void OpenCalendar_Click(object sender, RoutedEventArgs e)
        {
            cld.Visibility = Visibility.Visible;
        }

        private async void cld_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadWallpaper(cld.SelectedDate);
        }

        private void cld_MouseLeave(object sender, MouseEventArgs e)
        {
            cld.Visibility = Visibility.Hidden;
        }
    }
}
