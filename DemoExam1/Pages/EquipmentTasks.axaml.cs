using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DemoExam1.Pages
{
    public partial class EquipmentTasks : Window
    {
        public EquipmentTasks()
        {
            InitializeComponent();
            TasksDataGrid.ItemsSource = App.DbContext!.Tasks.ToList();
        }
    }
}