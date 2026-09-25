using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DemoExam1.Data.Models;

namespace DemoExam1.Pages
{
    public partial class EquipmentTasks : Window
    {
        public EquipmentTasks()
        {
            InitializeComponent();
            TasksDataGrid.ItemsSource = App.DbContext!.Tasks.ToList();
            TasksDataGrid.Columns[^1].IsVisible = App.CurrentUser.UserRole == UserRole.Specialist || App.CurrentUser.UserRole == UserRole.Admin;
        }

        private void TakeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not RepairTask task)
                return;

            task.Status = TaskStatus.Working;
            task.DateOfStartWork = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            App.DbContext!.SaveChanges();
            TasksDataGrid.ItemsSource = App.DbContext.Tasks.ToList();
        }

        private void LoadButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void LoadTasks()
        {
            TasksDataGrid.ItemsSource = App.DbContext!.Tasks.ToList();
        }
    }
}