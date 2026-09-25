using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DemoExam1.Data;
using DemoExam1.Data.Models;
using DemoExam1.Pages;
using Microsoft.EntityFrameworkCore;

namespace DemoExam1;

public partial class App : Application
{
    public static AppDbContext? DbContext { get; private set; }
    public static User CurrentUser { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Design.IsDesignMode)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        DbContext = new AppDbContext();

        DbContext.Database.Migrate();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            CurrentUser = DbContext.User.FirstOrDefault(u => u.UserRole == UserRole.Admin);

            if (CurrentUser != null)
            {
                desktop.MainWindow = new MainWindow();
            }
            else
            {
                desktop.MainWindow = new LoginPage();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}