using Microsoft.WindowsAPICodePack.Dialogs;
using ScreenRecorder.FinalVer.CloudStorage;
using ScreenRecorder.FinalVer.Properties;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenRecorder.FinalVer
{
    /// <summary>
    /// Логика взаимодействия для StorageViewWindow.xaml
    /// </summary>
    public partial class StorageViewWindow : Window
    {
        StorageAgent agent;

        public StorageViewWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void UploadFileNames()
        {
            DataTable data = agent.LoadFileNames();
            StorageView.ItemsSource = data.DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            agent = new StorageAgent();
            UploadFileNames();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            //dialog
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.ShowDialog();

            //load selected data

            object item = StorageView.SelectedItem;
            if (item != null)
            {
                string content = (StorageView.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                //downloading
                agent.DownloadAsync(dialog.FileName + "\\" + content, $@"user-{Settings.Default.UserId}/" + content);
                MessageBox.Show($"Файл {content} успешно сохранен в папке {dialog.FileName}");
            }
            else
            {
                MessageBox.Show("Занчение не выделено");
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object item = StorageView.SelectedItem;
            if (item != null)
            {
                string content = (StorageView.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                //downloading
                agent.Delete($@"user-{Settings.Default.UserId}/" + content);
                MessageBox.Show($"Файл {content} был удален");
                UploadFileNames();
            }
            else
            {
                MessageBox.Show("Занчение не выделено");
            }
        }
    }
}
