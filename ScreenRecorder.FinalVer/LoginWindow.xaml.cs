using ScreenRecorder.FinalVer.DataBase;
using ScreenRecorder.FinalVer.Properties;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ScreenRecorder.FinalVer
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        Dictionary<string, string> LogPassContainer;
        Dictionary<string, Guid> LogIDContainer;

        DataBaseAgent agent = new DataBaseAgent();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void CloseLogin_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            LogIDContainer = new Dictionary<string, Guid>();
            LogPassContainer = new Dictionary<string, string>();

            var datas = agent.LoadColumnData(new DataBaseContext().Accounts);
            foreach (var item in datas)
            {
                var account = item as Account;
                LogIDContainer.Add(account.Username, account.ID);
                LogPassContainer.Add(account.Username, account.Password);
            }
            LoginText.ItemsSource = LogPassContainer.Keys;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (BCrypt.Net.BCrypt.Verify(PasswordText.Password, LogPassContainer[LoginText.Text]))
            {
                Settings.Default.UserId = LogIDContainer[LoginText.Text];
                Settings.Default.IsLogged = true;
                MessageBox.Show($"Пользователь {LoginText.Text} успешно авторизован", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show($"Пользователь {LoginText.Text} не зарегестрирован в базе данных", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ReisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!agent.UploadAcconutData(LoginText.Text, BCrypt.Net.BCrypt.HashPassword(PasswordText.Password)))
            {
                MessageBox.Show($"Пользователь с именем {LoginText.Text} уже зарегестрирован в базе данных", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Update();
        }
    }
}
