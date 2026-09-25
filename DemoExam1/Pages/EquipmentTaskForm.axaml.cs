using System;
using System.Linq;
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
            ClientsBox.ItemsSource = App.DbContext!.Client.OrderBy(c => c.Name).ToList();
            EquipmentBox.ItemsSource = App.DbContext.Equipment.OrderBy(e => e.SerialEquipment).ToList();
        }

        private void Register_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ClientsBox.SelectedItem is not Client client)
            {
                ErrorBox.Text = "Выберите клиента. Если список пуст, сначала добавьте его в разделе «Клиенты».";
                return;
            }

            if (!TryGetEquipment(out var equipment))
                return;

            var selected = RegistrationDatePicker.SelectedDate ?? DateTime.Now;

            var task = new RepairTask
            {
                Priority = PriorityBox.Text ?? "",
                Status = TaskStatus.Registered,
                EquipmentProblem = ProblemBox.Text ?? "",
                EquipmentTypeOfProblem = ProblemTypeBox.Text ?? "",
                ClientId = client.Id,
                ClientComment = ClientCommentBox.Text,
                Equipment = equipment,
                DateOfRegistration = ToUtc(selected)
            };

            try
            {
                App.DbContext!.Tasks.Add(task);
                App.DbContext.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                ErrorBox.Text = ex.GetBaseException().Message;
            }
        }

        private bool TryGetEquipment(out Equipment equipment)
        {
            if (EquipmentBox.SelectedItem is Equipment selected)
            {
                equipment = selected;
                return true;
            }

            var serial = SerialBox.Text?.Trim() ?? "";
            var type = EquipmentTypeBox.Text?.Trim() ?? "";
            if (serial.Length == 0 || serial.Length > 12 || string.IsNullOrWhiteSpace(type))
            {
                ErrorBox.Text = "Выберите оборудование или укажите серийный номер (до 12 символов) и тип.";
                equipment = null!;
                return false;
            }

            equipment = App.DbContext!.Equipment.FirstOrDefault(item => item.SerialEquipment == serial)
                ?? new Equipment { SerialEquipment = serial, TypeEquipment = type };
            return true;
        }

        private static DateTime ToUtc(DateTime value) =>
            value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime()
            };
    }
}