using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InvoicesDesktop.Converters;

/// <summary>
/// Returns <see cref="Visibility.Visible"/> when the bound integer count is zero
/// (used to show empty-state placeholders), otherwise <see cref="Visibility.Collapsed"/>.
/// </summary>
public class CountToVisibilityConverter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var count = value is int i ? i : 0;
        return count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
