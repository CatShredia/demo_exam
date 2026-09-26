using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace DemoExam1.Converters;

public class LocalTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime date)
            return "";

        var utc = date.Kind == DateTimeKind.Local
            ? date.ToUniversalTime()
            : DateTime.SpecifyKind(date, DateTimeKind.Utc);

        return utc.ToLocalTime().ToString("dd.MM.yyyy HH:mm", culture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;
}
