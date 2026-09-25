using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using DemoExam1.Data.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace DemoExam1.Pages
{
    public partial class TaskDetailsWindow : Window
    {
        private readonly int _taskId;

        public TaskDetailsWindow()
            : this(0)
        {
        }

        public TaskDetailsWindow(int taskId)
        {
            InitializeComponent();
            _taskId = taskId;
            if (taskId == 0 || App.DbContext == null)
                return;

            ApplyRole();
            ShowTask();
        }

        private void ApplyRole()
        {
            var role = App.CurrentUser.UserRole;
            var isAdmin = role == UserRole.Admin;
            var isManager = role == UserRole.Manager;
            var isSpecialist = role == UserRole.Specialist;

            SaveDescriptionButton.IsVisible = isAdmin || isManager || isSpecialist;
            AddSpecialistButton.IsVisible = isAdmin || isManager;
            RemoveSpecialistButton.IsVisible = isAdmin || isManager;
            SpecialistBox.IsVisible = isAdmin || isManager;

            var worker = isAdmin || isSpecialist;
            TakeButton.IsVisible = worker;
            CancelButton.IsVisible = worker || isManager;
            FinishButton.IsVisible = worker;
            SaveReportButton.IsVisible = worker;
            AddSpareButton.IsVisible = worker;
            RemoveSpareButton.IsVisible = worker;
            AddLineButton.IsVisible = isAdmin || isManager || isSpecialist;
            RemoveLineButton.IsVisible = isAdmin || isManager;
        }

        private void SaveDescription_Click(object? sender, RoutedEventArgs e)
        {
            var task = FindTask();
            if (task == null)
                return;

            task.Priority = PriorityBox.Text?.Trim() ?? "";
            task.ClientComment = CommentBox.Text;
            task.EquipmentProblem = ProblemBox.Text?.Trim() ?? "";
            task.EquipmentTypeOfProblem = ProblemTypeBox.Text?.Trim() ?? "";
            if (ClientsBox.SelectedItem is Client client)
                task.ClientId = client.Id;

            Save(task);
        }

        private void AddSpecialist_Click(object? sender, RoutedEventArgs e)
        {
            if (SpecialistBox.SelectedItem is not User selected)
            {
                ErrorText.Text = "Выберите специалиста.";
                return;
            }

            var task = FindTask();
            if (task == null)
                return;

            task.Specialists ??= new System.Collections.Generic.List<User>();
            if (task.Specialists.All(s => s.Id != selected.Id))
                task.Specialists.Add(selected);
            Save(task);
        }

        private void RemoveSpecialist_Click(object? sender, RoutedEventArgs e)
        {
            if (SpecialistsList.SelectedItem is not User selected)
            {
                ErrorText.Text = "Выберите специалиста в списке.";
                return;
            }

            var task = FindTask();
            if (task == null)
                return;

            var current = task.Specialists?.FirstOrDefault(s => s.Id == selected.Id);
            if (current != null)
                task.Specialists!.Remove(current);
            Save(task);
        }

        private void Take_Click(object? sender, RoutedEventArgs e) => SetStatus(TaskStatus.Working);

        private void Cancel_Click(object? sender, RoutedEventArgs e) => SetStatus(TaskStatus.Closed);

        private void Finish_Click(object? sender, RoutedEventArgs e) => SetStatus(TaskStatus.Finished);

        private void SaveReport_Click(object? sender, RoutedEventArgs e)
        {
            var task = FindTask();
            if (task == null)
                return;

            if (!TryDecimal(HoursBox.Text, out var hours) && !string.IsNullOrWhiteSpace(HoursBox.Text))
            {
                ErrorText.Text = "Часы нужно указать числом.";
                return;
            }

            task.WorkHours = string.IsNullOrWhiteSpace(HoursBox.Text) ? null : hours;
            task.SpecialistReport = ReportBox.Text;
            Save(task);
        }

        private void AddSpare_Click(object? sender, RoutedEventArgs e)
        {
            var name = SpareNameBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorText.Text = "Введите название материала.";
                return;
            }

            if (!TryDecimal(ExpectedCostBox.Text, out var expected))
                expected = 0;
            decimal? total = null;
            if (!string.IsNullOrWhiteSpace(TotalCostBox.Text))
            {
                if (!TryDecimal(TotalCostBox.Text, out var parsed))
                {
                    ErrorText.Text = "Стоимость нужно указать числом.";
                    return;
                }

                total = parsed;
            }

            DateTime? received = null;
            if (GettingDatePicker.SelectedDate is DateTime selected)
                received = TaskInfo.FromPicker(selected);

            App.DbContext!.OrderSpare.Add(new OrderSpare
            {
                NameSpare = name,
                ExpectedCost = expected,
                TotalCost = total,
                TaskId = _taskId,
                DateOfOrder = DateTime.UtcNow,
                DateOfGetting = received
            });
            App.DbContext.SaveChanges();
            SpareNameBox.Text = "";
            ExpectedCostBox.Text = "";
            TotalCostBox.Text = "";
            GettingDatePicker.SelectedDate = null;
            ErrorText.Text = "";
            ShowTask();
        }

        private void RemoveSpare_Click(object? sender, RoutedEventArgs e)
        {
            if (SparesGrid.SelectedItem is not SpareRow row)
            {
                ErrorText.Text = "Выберите материал в таблице.";
                return;
            }

            var spare = App.DbContext!.OrderSpare.FirstOrDefault(s => s.Id == row.Id);
            if (spare != null)
            {
                App.DbContext.OrderSpare.Remove(spare);
                App.DbContext.SaveChanges();
            }

            ErrorText.Text = "";
            ShowTask();
        }

        private void AddLine_Click(object? sender, RoutedEventArgs e)
        {
            var serial = SerialBox.Text?.Trim() ?? "";
            var type = EquipmentTypeBox.Text?.Trim() ?? "";
            if (serial.Length == 0 || serial.Length > 12 || string.IsNullOrWhiteSpace(type))
            {
                ErrorText.Text = "Укажите серийный номер (до 12 символов) и тип оборудования.";
                return;
            }

            var equipment = App.DbContext!.Equipment.FirstOrDefault(x => x.SerialEquipment == serial);
            if (equipment == null)
            {
                equipment = new Equipment { SerialEquipment = serial, TypeEquipment = type };
                App.DbContext.Equipment.Add(equipment);
                App.DbContext.SaveChanges();
            }

            var task = FindTask();
            if (task == null)
                return;

            task.EquipmentLines.Add(new TaskEquipment
            {
                EquipmentId = equipment.Id,
                Problem = LineProblemBox.Text?.Trim() ?? "",
                TypeOfProblem = LineProblemTypeBox.Text?.Trim() ?? ""
            });
            Save(task);
            SerialBox.Text = "";
            EquipmentTypeBox.Text = "";
            LineProblemBox.Text = "";
            LineProblemTypeBox.Text = "";
        }

        private void RemoveLine_Click(object? sender, RoutedEventArgs e)
        {
            if (LinesGrid.SelectedItem is not LineRow row)
            {
                ErrorText.Text = "Выберите оборудование в таблице.";
                return;
            }

            var line = App.DbContext!.TaskEquipment.FirstOrDefault(x => x.Id == row.Id);
            if (line != null)
            {
                App.DbContext.TaskEquipment.Remove(line);
                App.DbContext.SaveChanges();
            }

            ErrorText.Text = "";
            ShowTask();
        }

        private void SetStatus(TaskStatus status)
        {
            var task = FindTask();
            if (task == null)
                return;

            task.Status = status;
            if (status == TaskStatus.Working)
            {
                task.DateOfStartWork ??= DateTime.UtcNow;
                if (App.CurrentUser.UserRole == UserRole.Specialist)
                {
                    task.Specialists ??= new System.Collections.Generic.List<User>();
                    if (task.Specialists.All(s => s.Id != App.CurrentUser.Id))
                    {
                        var me = App.DbContext!.User.First(u => u.Id == App.CurrentUser.Id);
                        task.Specialists.Add(me);
                    }
                }
            }

            if (status is TaskStatus.Closed or TaskStatus.Finished)
                task.DateOfClose = DateTime.UtcNow;

            Save(task);
        }

        private void Save(RepairTask task)
        {
            try
            {
                TaskInfo.NormalizeDates(task);
                App.DbContext!.SaveChanges();
                ErrorText.Text = "";
                ShowTask();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.GetBaseException().Message;
            }
        }

        private RepairTask? FindTask()
        {
            var task = App.DbContext!.Tasks.FirstOrDefault(t => t.Id == _taskId);
            if (task == null)
            {
                ErrorText.Text = "Заявка не найдена.";
                return null;
            }

            var entry = App.DbContext.Entry(task);
            entry.Reference(t => t.Client).Load();
            entry.Reference(t => t.Equipment).Load();
            entry.Collection(t => t.Specialists).Load();
            entry.Collection(t => t.EquipmentLines).Query().Include(x => x.Equipment).Load();
            task.Specialists ??= new System.Collections.Generic.List<User>();
            task.EquipmentLines ??= new System.Collections.Generic.List<TaskEquipment>();
            return task;
        }

        private void ShowTask()
        {
            var task = FindTask();
            if (task == null)
                return;

            HeaderText.Text = "Заявка №" + task.Id;
            StatusText.Text = TaskInfo.StatusTitle(task.Status)
                + "    Регистрация: " + TaskInfo.Local(task.DateOfRegistration)
                + "    Начало: " + TaskInfo.Local(task.DateOfStartWork)
                + "    Закрытие: " + TaskInfo.Local(task.DateOfClose);

            LegacyEquipmentText.Text = task.Equipment == null
                ? ""
                : "Оборудование из регистрации: " + task.Equipment.SerialEquipment + ", " + task.Equipment.TypeEquipment;

            PriorityBox.Text = task.Priority;
            CommentBox.Text = task.ClientComment;
            ProblemBox.Text = task.EquipmentProblem;
            ProblemTypeBox.Text = task.EquipmentTypeOfProblem;
            HoursBox.Text = task.WorkHours?.ToString(CultureInfo.CurrentCulture) ?? "";
            ReportBox.Text = task.SpecialistReport;

            var clients = App.DbContext!.Client.OrderBy(c => c.Name).ToList();
            ClientsBox.ItemsSource = clients;
            ClientsBox.SelectedItem = clients.FirstOrDefault(c => c.Id == task.ClientId);

            var specialists = App.DbContext.User
                .Where(u => u.UserRole == UserRole.Specialist)
                .OrderBy(u => u.Login)
                .ToList();
            SpecialistBox.ItemsSource = specialists;
            SpecialistsList.ItemsSource = task.Specialists.OrderBy(s => s.Login).ToList();

            SparesGrid.ItemsSource = App.DbContext.OrderSpare
                .Where(s => s.TaskId == task.Id)
                .OrderBy(s => s.Id)
                .AsEnumerable()
                .Select(s => new SpareRow
                {
                    Id = s.Id,
                    Name = s.NameSpare,
                    Expected = s.ExpectedCost.ToString("0.##"),
                    Total = s.TotalCost?.ToString("0.##") ?? "",
                    Ordered = TaskInfo.Local(s.DateOfOrder),
                    Received = TaskInfo.Local(s.DateOfGetting)
                })
                .ToList();

            LinesGrid.ItemsSource = task.EquipmentLines
                .OrderBy(l => l.Id)
                .Select(l => new LineRow
                {
                    Id = l.Id,
                    Serial = l.Equipment.SerialEquipment,
                    Type = l.Equipment.TypeEquipment,
                    Problem = l.Problem,
                    ProblemType = l.TypeOfProblem
                })
                .ToList();

            ShowQr(task);
        }

        private void ShowQr(RepairTask task)
        {
            var text = "Заявка " + task.Id + "; " + TaskInfo.StatusTitle(task.Status) + "; " + task.Priority;
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data).GetGraphic(6);
            QrImage.Source = new Bitmap(new MemoryStream(png));
        }

        private static bool TryDecimal(string? text, out decimal value)
        {
            text = text?.Trim();
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value)
                || decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private sealed class SpareRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Expected { get; set; } = "";
            public string Total { get; set; } = "";
            public string Ordered { get; set; } = "";
            public string Received { get; set; } = "";
        }

        private sealed class LineRow
        {
            public int Id { get; set; }
            public string Serial { get; set; } = "";
            public string Type { get; set; } = "";
            public string Problem { get; set; } = "";
            public string ProblemType { get; set; } = "";
        }
    }
}
