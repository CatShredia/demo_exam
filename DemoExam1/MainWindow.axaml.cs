using Avalonia.Controls;
using DemoExam1.Pages;

namespace DemoExam1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        HelloBox.Text = "Привет " + App.CurrentUser.Login + "!    " + App.CurrentUser.UserRole;
    }

    private void ShowEquipment(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new EquipmentTasks();
        mainWindow.Show();
        this.Close();
    }
}