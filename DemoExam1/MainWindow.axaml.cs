using Avalonia.Controls;
using DemoExam1.Data.Models;
using DemoExam1.Pages;

namespace DemoExam1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        HelloBox.Text = "Привет " + App.CurrentUser.Login + "!    " + App.CurrentUser.UserRole;

        var role = App.CurrentUser.UserRole;
        ClientsButton.IsVisible = role is UserRole.Admin or UserRole.Manager;
        UsersButton.IsVisible = role == UserRole.Admin;
    }

    private void ShowEquipment(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new EquipmentTasks();
        mainWindow.Show();
    }

    private void ShowRegisterOfTask(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new EquipmentTaskForm();
        mainWindow.Show();
    }

    private void ShowTasks(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new TaskListWindow().Show();
    }

    private void ShowClients(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new ClientsWindow().Show();
    }

    private void ShowUsers(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new UsersWindow().Show();
    }

    private void ShowMonitoring(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new MonitoringWindow().Show();
    }
}