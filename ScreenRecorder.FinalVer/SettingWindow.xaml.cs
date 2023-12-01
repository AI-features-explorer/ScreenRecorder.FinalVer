using Microsoft.WindowsAPICodePack.Dialogs;
using ScreenRecorder.FinalVer.Properties;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenRecorder.FinalVer
{
    /// <summary>
    /// Логика взаимодействия для SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public CSScreenRecorder.ScreenRecorder ScreenRecorder { get; private set; }

        public SettingWindow()
        {
            InitializeComponent();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (ScreenRecorder = new CSScreenRecorder.ScreenRecorder("UserName", "RegKey"))
            {
                #region Load Audio Properties

                IsAudioRecodingMode.IsChecked = Settings.Default.IsAudioRecord;

                //Bitrate
                BitrateBox.ItemsSource = ScreenRecorder.GetAudioBitrates();
                BitrateBox.Text = Settings.Default.AudioBitrate;

                //Samplerate
                SamplerateBox.ItemsSource = ScreenRecorder.GetAudioSamplerates();
                SamplerateBox.Text = Settings.Default.AudioSamplerate;

                //Bit depth
                BitDepthBox.ItemsSource = ScreenRecorder.GetAudioBits();
                BitDepthBox.Text = Settings.Default.AudioBits;

                //Channels
                ChannnelsBox.ItemsSource = ScreenRecorder.GetAudioChannels();
                ChannnelsBox.Text = Settings.Default.AudioChannels;

                //Recorder mode
                RecorderModeBox.ItemsSource = ScreenRecorder.GetAudioModes();
                RecorderModeBox.Text = Settings.Default.AudioMode;

                #endregion

                #region Load App Properties

                //Directory
                DirectoryText.Text = Settings.Default.Directory;

                //Track mouse
                TrackMouseMode.IsChecked = Settings.Default.IsTrackMouse;
                #endregion

            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void PercentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PescentText != null)
                PescentText.Text = PercentSlider.Value + "%"; ;
        }

        private void RecorderModeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (ScreenRecorder = new CSScreenRecorder.ScreenRecorder("UserName", "RegKey"))
            {
                CSScreenAudioRecorder.Mode mode = (CSScreenAudioRecorder.Mode)Enum.Parse(typeof(CSScreenAudioRecorder.Mode), RecorderModeBox.Text);

                AudioSourceBox.ItemsSource = ScreenRecorder.GetDevices(mode);

                int default_device_index = ScreenRecorder.GetDeviceDefaultIndex(mode);

                if (default_device_index != -1)
                    AudioSourceBox.SelectedIndex = default_device_index;
                else
                    AudioSourceBox.SelectedIndex = 0;

            }
        }

        private void CloseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TempFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.ShowDialog();
            DirectoryText.Text = dialog.FileName;

        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            #region Load Audio Properties

            Settings.Default.IsAudioRecord = IsAudioRecodingMode.IsChecked.Value;

            //Bitrate
            Settings.Default.AudioBitrate = BitrateBox.Text;

            //Samplerate
            Settings.Default.AudioSamplerate = SamplerateBox.Text;

            //Bit depth
            Settings.Default.AudioBits = BitDepthBox.Text;

            //Channels
            Settings.Default.AudioChannels = ChannnelsBox.Text;

            //Recorder mode
            Settings.Default.AudioMode = RecorderModeBox.Text;

            #endregion
            #region Load App Properties

            //Directory
            Settings.Default.Directory = DirectoryText.Text;

            //Track mouse
            Settings.Default.IsTrackMouse = TrackMouseMode.IsChecked.Value;

            //Device
            Settings.Default.AudioDeviceIndex = AudioSourceBox.SelectedIndex;

            Settings.Default.Save();
            #endregion
            Close();
        }
    }
}
