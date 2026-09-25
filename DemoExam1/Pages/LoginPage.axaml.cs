using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DemoExam1.Data.Models;

namespace DemoExam1.Pages
{
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var login = LoginBox.Text?.Trim();
            var password = PasswordBox.Text ?? "";

            if (login == null || password == null)
            {
                ErrorBox.Text = "Введите все значения!";
            }
            else
            {

                var user = App.DbContext!.User.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    App.CurrentUser = user;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorBox.Text = "Неверные логин или пароль";
                }
            }
        }
    }
}