using System.Linq;
using Avalonia.Controls;
using DemoExam1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoExam1.Pages
{
    public partial class MonitoringWindow : Window
    {
        public MonitoringWindow()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            var onlyMine = App.CurrentUser.UserRole == UserRole.Specialist;
            ScopeText.Text = onlyMine
                ? "Показатели по вашим заявкам."
                : "Показатели по всем заявкам.";

            var query = App.DbContext!.Tasks
                .Include(t => t.EquipmentLines)
                .AsQueryable();

            if (onlyMine)
            {
                var userId = App.CurrentUser.Id;
                query = query.Where(t => t.Specialists.Any(s => s.Id == userId));
            }

            var tasks = query.ToList();
            var done = tasks
                .Where(t => t.Status == TaskStatus.Finished && t.DateOfClose != null)
                .ToList();

            CompletedText.Text = "Выполнено заявок: " + done.Count;
            if (done.Count == 0)
            {
                AverageText.Text = "Среднее время выполнения: —";
            }
            else
            {
                var hours = done.Average(t =>
                    (t.DateOfClose!.Value - (t.DateOfStartWork ?? t.DateOfRegistration)).TotalHours);
                AverageText.Text = "Среднее время выполнения: " + hours.ToString("0.0") + " ч";
            }

            var types = tasks
                .Select(t => t.EquipmentTypeOfProblem)
                .Concat(tasks.SelectMany(t => t.EquipmentLines).Select(l => l.TypeOfProblem))
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .GroupBy(t => t)
                .Select(g => new StatRow { Type = g.Key, Count = g.Count() })
                .OrderByDescending(r => r.Count)
                .ToList();

            StatsGrid.ItemsSource = types;
        }

        private sealed class StatRow
        {
            public string Type { get; set; } = "";
            public int Count { get; set; }
        }
    }
}
