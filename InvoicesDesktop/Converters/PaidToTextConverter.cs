using System.Globalization;
using System.Windows.Data;

namespace InvoicesDesktop.Converters;

/// <summary>
/// Converts an invoice paid flag into a human-readable status label.
/// </summary>
public class PaidToTextConverter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? "PAID" : "UNPAID";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
