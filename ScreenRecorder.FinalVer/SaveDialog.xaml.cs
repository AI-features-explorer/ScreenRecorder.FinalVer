using Microsoft.WindowsAPICodePack.Dialogs;
using ScreenRecorder.FinalVer.CloudStorage;
using ScreenRecorder.FinalVer.Properties;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ScreenRecorder.FinalVer
{
    /// <summary>
    /// Логика взаимодействия для SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : Window
    {
        public SaveDialog()
        {
            InitializeComponent();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveToDrive_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.ShowDialog();
            File.Move(Settings.Default.Filename, dialog.FileName + "\\" + System.IO.Path.GetFileName(Settings.Default.Filename));
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private async void SaveInCloud_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsLogged)
            {
                StorageAgent agent = new StorageAgent();
                await agent.UploadAsync(Settings.Default.Filename);
                File.Delete(Settings.Default.Filename);
                Close();
            }
            else
            {
                MessageBox.Show("вы не вошли");
            }
        }
    }
}
