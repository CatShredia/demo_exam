using System;
using DemoExam1.Data.Models;

namespace DemoExam1;

public static class TaskInfo
{
    public static string StatusTitle(TaskStatus status) => status switch
    {
        TaskStatus.Registered => "Зарегистрирована",
        TaskStatus.Working => "Взята в работу",
        TaskStatus.Closed => "Отменена",
        TaskStatus.Finished => "Выполнена",
        _ => status.ToString()
    };

    public static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    public static DateTime FromPicker(DateTime value)
    {
        var local = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Local)
            : value;
        return local.Kind == DateTimeKind.Utc ? local : local.ToUniversalTime();
    }

    public static string Local(DateTime value) =>
        AsUtc(value).ToLocalTime().ToString("dd.MM.yyyy HH:mm");

    public static string Local(DateTime? value) =>
        value is DateTime date ? Local(date) : "";

    public static void NormalizeDates(RepairTask task)
    {
        task.DateOfRegistration = AsUtc(task.DateOfRegistration);
        if (task.DateOfStartWork is DateTime start)
            task.DateOfStartWork = AsUtc(start);
        if (task.DateOfClose is DateTime close)
            task.DateOfClose = AsUtc(close);
    }
}
