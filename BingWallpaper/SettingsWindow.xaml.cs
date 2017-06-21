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
using System.Windows.Shapes;

namespace BingWallpaper
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            tbSave.Text = ConfigManager.Config.SaveDirectory;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                SelectedPath = ConfigManager.Config.SaveDirectory
            };

            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                tbSave.Text = dialog.SelectedPath;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var text = tbSave.Text.Trim();
            if (text.Length == 0)
            {
                MessageBox.Show("收藏夹不能为空");
                return;
            }
            ConfigManager.Config.SaveDirectory = text;
            this.DialogResult = true;
        }

        private void TSCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Scheduler.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void TSOpen_Click(object sender, RoutedEventArgs e)
        {
            Scheduler.Open();
        }

        private void TSDelete_Click(object sender, RoutedEventArgs e)
        {
            Scheduler.Delete();
        }
    }
}
