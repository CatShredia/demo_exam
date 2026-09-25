using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DemoExam1.Data.Models;

namespace DemoExam1.Pages
{
    public partial class ClientsWindow : Window
    {
        public ClientsWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void Add_Click(object? sender, RoutedEventArgs e)
        {
            var name = NameBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorText.Text = "Введите имя клиента.";
                return;
            }

            App.DbContext!.Client.Add(new Client { Name = name });
            App.DbContext.SaveChanges();
            NameBox.Text = "";
            ErrorText.Text = "";
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsGrid.ItemsSource = App.DbContext!.Client
                .OrderBy(c => c.Name)
                .ToList();
        }
    }
}
