using ScreenRecorder.FinalVer.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ScreenRecorder.FinalVer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int AreaHeight { get; private set; }
        public int AreaWidth { get; private set; }
        public int AreaTop { get; private set; }
        public int AreaLeft { get; private set; }

        public MainWindow()
        {
            Settings.Default.IsLogged = false;
            InitializeComponent();
        }

        CSScreenRecorder.ScreenRecorder ScreenRecorder;

        private void ScreenrecordBtn_Click(object sender, RoutedEventArgs e)
        {
            using (ScreenRecorder = new CSScreenRecorder.ScreenRecorder("UserName", "RegKey"))
            {

                #region Area

                ScreenRecorder.RecordScreenHeight = AreaHeight;
                ScreenRecorder.RecordScreenWidth = AreaWidth;
                ScreenRecorder.RecordScreenTop = AreaTop;
                ScreenRecorder.RecordScreenLeft = AreaLeft;

                #endregion

                #region Video

                ScreenRecorder.VideoFPS = 24;
                ScreenRecorder.VideoEncoderQuality = Settings.Default.VideoEncoderQuality; //%
                ScreenRecorder.VideoCodecIndex = 1; //jpeg commpression

                #endregion Video

                #region Audio

                ScreenRecorder.AudioRecorder = true;
                ScreenRecorder.AudioDeviceIndex = Settings.Default.AudioDeviceIndex;
                ScreenRecorder.AudioMode = (CSScreenAudioRecorder.Mode)Enum.Parse(typeof(CSScreenAudioRecorder.Mode), Settings.Default.AudioMode);
                ScreenRecorder.AudioSamplerate = (CSScreenAudioRecorder.Samplerate)Enum.Parse(typeof(CSScreenAudioRecorder.Samplerate), Settings.Default.AudioSamplerate);
                ScreenRecorder.AudioBits = (CSScreenAudioRecorder.Bits)Enum.Parse(typeof(CSScreenAudioRecorder.Bits), Settings.Default.AudioBits);
                ScreenRecorder.AudioChannels = (CSScreenAudioRecorder.Channels)Enum.Parse(typeof(CSScreenAudioRecorder.Channels), Settings.Default.AudioChannels);
                ScreenRecorder.AudioFormat = (CSScreenAudioRecorder.Format)Enum.Parse(typeof(CSScreenAudioRecorder.Format), "WAV");
                ScreenRecorder.AudioBitrate = (CSScreenAudioRecorder.Bitrate)Enum.Parse(typeof(CSScreenAudioRecorder.Bitrate), Settings.Default.AudioBitrate);
                
                #endregion Audio

                ScreenRecorder.TrackMouse = true;

                #region Events

                ScreenRecorder.OnStartRecord += (o) =>
                {
                    LogText.Text = LogText.Text + DateTime.Now + ": Начало записи\r\n";
                };

                ScreenRecorder.OnPauseRecord += (o) =>
                {
                    LogText.Text = LogText.Text + DateTime.Now + ": Запись приотсановлена\r\n";
                };

                ScreenRecorder.OnUnPauseRecord += (o) =>
                {
                    LogText.Text = LogText.Text + DateTime.Now + ": Запись возобновлена\r\n";
                };

                ScreenRecorder.OnStopRecord += (o) =>
                {
                    LogText.Text = LogText.Text + DateTime.Now + ": Запись завершена\r\n";
                };

                ScreenRecorder.OnPreview += (o, bmp) =>
                {
                    //nop
                };

                ScreenRecorder.OnError += (o, ErrorMessage, ErrorNumber) =>
                {
                    MessageBox.Show(ErrorMessage + " (" + ErrorNumber + ")", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogText.Text = LogText.Text + DateTime.Now + ": Ошибка - " + ErrorMessage + " (" + ErrorNumber + ")\r\n";
                };

                #endregion

                ScreenRecorder.FileName = $"{Settings.Default.Directory}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.avi"; ;
                Settings.Default.Filename = ScreenRecorder.FileName;
                ScreenRecorder.Record();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            ClaculateResolutionWithDPI(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight, 0, 0);
            LogText.Text = DateTime.Now + $": Текущее разрешение области захвата = {AreaWidth}x{AreaHeight}" + Environment.NewLine;
        }

        private void ClaculateResolutionWithDPI(double width, double heigth, double top, double left)
        {
            var dpi = System.Windows.Media.VisualTreeHelper.GetDpi(new System.Windows.Controls.Control());
            AreaWidth = (int)(width * dpi.DpiScaleX);
            AreaHeight = (int)(heigth * dpi.DpiScaleY);
            AreaLeft = (int)(top * dpi.DpiScaleX);
            AreaTop = (int)(left * dpi.DpiScaleY);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScreenRecorder.Stop();
                SaveDialog saveDialog = new SaveDialog();
                saveDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenRecorder.Pause();
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenRecorder.UnPause();
        }

        private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            Screenshot();
        }

        private void Screenshot()
        {
            using (Bitmap bmp = new Bitmap(AreaWidth, AreaHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    string filename = $"ScreenShot-{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png";
                    g.CopyFromScreen(AreaLeft, AreaTop, 0, 0, bmp.Size);
                    bmp.Save($"{Settings.Default.Directory}\\{filename}", ImageFormat.Png);
                    Settings.Default.Filename = $"{Settings.Default.Directory}\\{filename}";
                }
                SaveDialog dialog = new SaveDialog();
                dialog.ShowDialog();
            }
        }

        private int CoordinatCorrecting(int coodinat)
        {
            return coodinat % 2 != 0 ? coodinat+1 : coodinat; 
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            CSScreenRecorder.SpinningSelection SpinningSelection = new CSScreenRecorder.SpinningSelection();

            using (ScreenRecorder = new CSScreenRecorder.ScreenRecorder("UserName", "RegKey"))
            {

                ScreenRecorder.SpinningToolColor = System.Drawing.Color.Orange;
                ScreenRecorder.SpinningToolBorderSize = 3;
                ScreenRecorder.SpinningToolStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                ScreenRecorder.OnSpinningSelection += (o, Cancel, Top, Left, Width, Height) =>
                {

                    if (Cancel)
                    {
                        LogText.Text = LogText.Text + DateTime.Now + ": Выделение отменено\r\n";
                    }
                    else
                    {
                        int coodrdinat;
                        coodrdinat = CoordinatCorrecting(Top);
                        AreaTop = coodrdinat;
                        coodrdinat = CoordinatCorrecting(Left);
                        AreaLeft = coodrdinat;
                        coodrdinat = CoordinatCorrecting(Width);
                        AreaWidth = coodrdinat;
                        coodrdinat = CoordinatCorrecting(Height);
                        AreaHeight = coodrdinat;
                        LogText.Text += DateTime.Now + $": Текущее разрешение области захвата =  {AreaWidth}x{AreaHeight}" + Environment.NewLine;
                    }

                };

                SpinningSelection = ScreenRecorder.SpinningTool();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow();
            window.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void StorageButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsLogged)
            {
                StorageViewWindow window = new StorageViewWindow();
                window.Show();
            }
            else
            {
                MessageBox.Show("Вы не авторизованы", "Облако", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
