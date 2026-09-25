using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DemoExam1.Data.Models;

namespace DemoExam1.Pages
{
    public partial class EquipmentTaskForm : Window
    {
        public EquipmentTaskForm()
        {
            InitializeComponent();
        }

        private void Register_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int selectedClientId = 2;
            int selectedEquipmentId = 2;

            var selected = RegistrationDatePicker.SelectedDate ?? DateTime.UtcNow;

            var task = new RepairTask
            {
                Priority = PriorityBox.Text ?? "",
                Status = TaskStatus.Registered,
                EquipmentProblem = ProblemBox.Text ?? "",
                EquipmentTypeOfProblem = ProblemTypeBox.Text ?? "",
                ClientId = selectedClientId,
                ClientComment = ClientCommentBox.Text,
                EquipmentId = selectedEquipmentId,
                DateOfRegistration = DateTime.SpecifyKind(selected, DateTimeKind.Utc)
            };

            App.DbContext!.Tasks.Add(task);
            App.DbContext.SaveChanges();

            this.Close();
        }
    }
}