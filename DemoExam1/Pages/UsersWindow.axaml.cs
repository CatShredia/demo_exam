using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DemoExam1.Data.Models;

namespace DemoExam1.Pages
{
    public partial class UsersWindow : Window
    {
        public UsersWindow()
        {
            InitializeComponent();
            RoleBox.ItemsSource = new List<RoleChoice>
            {
                new(UserRole.Admin, "Администратор"),
                new(UserRole.Manager, "Менеджер"),
                new(UserRole.Specialist, "Специалист")
            };
            RoleBox.SelectedIndex = 2;
            LoadUsers();
        }

        private void Add_Click(object? sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text?.Trim();
            var password = PasswordBox.Text ?? "";
            var phone = PhoneBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Введите логин и пароль.";
                return;
            }

            if (App.DbContext!.User.Any(u => u.Login == login))
            {
                ErrorText.Text = "Такой логин уже есть.";
                return;
            }

            var role = (RoleBox.SelectedItem as RoleChoice)?.Role ?? UserRole.Specialist;
            App.DbContext.User.Add(new User
            {
                Login = login,
                Password = password,
                Phone = phone,
                UserRole = role
            });
            App.DbContext.SaveChanges();
            LoginBox.Text = "";
            PasswordBox.Text = "";
            PhoneBox.Text = "";
            ErrorText.Text = "";
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersGrid.ItemsSource = App.DbContext!.User
                .OrderBy(u => u.Login)
                .Select(u => new UserRow
                {
                    Login = u.Login,
                    Phone = u.Phone,
                    Role = u.UserRole == UserRole.Admin ? "Администратор"
                        : u.UserRole == UserRole.Manager ? "Менеджер"
                        : "Специалист"
                })
                .ToList();
        }

        private sealed class RoleChoice
        {
            public RoleChoice(UserRole role, string title)
            {
                Role = role;
                Title = title;
            }

            public UserRole Role { get; }
            public string Title { get; }
            public override string ToString() => Title;
        }

        private sealed class UserRow
        {
            public string Login { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Role { get; set; } = "";
        }
    }
}
