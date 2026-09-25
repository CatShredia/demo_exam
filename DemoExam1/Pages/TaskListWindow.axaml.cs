using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DemoExam1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoExam1.Pages
{
    public partial class TaskListWindow : Window
    {
        private readonly bool _onlyMine;

        public TaskListWindow()
        {
            InitializeComponent();
            _onlyMine = App.CurrentUser.UserRole == UserRole.Specialist;
            TitleText.Text = _onlyMine ? "Мои заявки" : "Заявки";
            ScopeText.Text = _onlyMine
                ? "Показаны только заявки, где вы указаны специалистом."
                : "Поиск по проблеме, приоритету, клиенту и статусу.";

            StatusBox.ItemsSource = new List<StatusChoice>
            {
                new(null, "Все статусы"),
                new(TaskStatus.Registered, TaskInfo.StatusTitle(TaskStatus.Registered)),
                new(TaskStatus.Working, TaskInfo.StatusTitle(TaskStatus.Working)),
                new(TaskStatus.Closed, TaskInfo.StatusTitle(TaskStatus.Closed)),
                new(TaskStatus.Finished, TaskInfo.StatusTitle(TaskStatus.Finished))
            };
            StatusBox.SelectedIndex = 0;
            LoadTasks();
        }

        private void Search_Click(object? sender, RoutedEventArgs e) => LoadTasks();

        private void Reset_Click(object? sender, RoutedEventArgs e)
        {
            ProblemBox.Text = "";
            PriorityBox.Text = "";
            ClientBox.Text = "";
            StatusBox.SelectedIndex = 0;
            LoadTasks();
        }

        private void Open_Click(object? sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not TaskRow row)
            {
                ErrorText.Text = "Выберите заявку в таблице.";
                return;
            }

            ErrorText.Text = "";
            new TaskDetailsWindow(row.Id).Show();
        }

        private void LoadTasks()
        {
            var problem = ProblemBox.Text?.Trim();
            var priority = PriorityBox.Text?.Trim();
            var client = ClientBox.Text?.Trim();
            var status = (StatusBox.SelectedItem as StatusChoice)?.Status;

            var query = App.DbContext!.Tasks
                .Include(t => t.Client)
                .Include(t => t.Specialists)
                .AsQueryable();

            if (_onlyMine)
            {
                var userId = App.CurrentUser.Id;
                query = query.Where(t => t.Specialists.Any(s => s.Id == userId));
            }

            if (!string.IsNullOrWhiteSpace(problem))
            {
                query = query.Where(t =>
                    t.EquipmentProblem.Contains(problem) ||
                    (t.ClientComment != null && t.ClientComment.Contains(problem)) ||
                    t.EquipmentLines.Any(l => l.Problem.Contains(problem)));
            }

            if (!string.IsNullOrWhiteSpace(priority))
                query = query.Where(t => t.Priority.Contains(priority));

            if (!string.IsNullOrWhiteSpace(client))
                query = query.Where(t => t.Client.Name.Contains(client));

            if (status != null)
                query = query.Where(t => t.Status == status);

            TasksGrid.ItemsSource = query
                .OrderByDescending(t => t.Id)
                .Select(t => new TaskRow
                {
                    Id = t.Id,
                    Priority = t.Priority,
                    Status = t.Status == TaskStatus.Registered ? "Зарегистрирована"
                        : t.Status == TaskStatus.Working ? "Взята в работу"
                        : t.Status == TaskStatus.Closed ? "Отменена"
                        : "Выполнена",
                    Client = t.Client.Name,
                    Problem = t.EquipmentProblem,
                    Registered = t.DateOfRegistration
                })
                .AsEnumerable()
                .Select(row =>
                {
                    row.RegisteredText = TaskInfo.Local(row.Registered);
                    return row;
                })
                .ToList();
        }

        private sealed class StatusChoice
        {
            public StatusChoice(TaskStatus? status, string title)
            {
                Status = status;
                Title = title;
            }

            public TaskStatus? Status { get; }
            public string Title { get; }
            public override string ToString() => Title;
        }

        private sealed class TaskRow
        {
            public int Id { get; set; }
            public string Priority { get; set; } = "";
            public string Status { get; set; } = "";
            public string Client { get; set; } = "";
            public string Problem { get; set; } = "";
            public System.DateTime Registered { get; set; }
            public string RegisteredText { get; set; } = "";
        }
    }
}
