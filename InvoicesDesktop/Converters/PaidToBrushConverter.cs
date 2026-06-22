using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InvoicesDesktop.Converters;

/// <summary>
/// Converts an invoice paid flag into a status chip background brush
/// (green for paid, amber for unpaid).
/// </summary>
public class PaidToBrushConverter : IValueConverter {

    private static readonly SolidColorBrush PaidBrush   = new(Color.FromRgb(0x43, 0xA0, 0x47));
    private static readonly SolidColorBrush UnpaidBrush = new(Color.FromRgb(0xFB, 0x8C, 0x00));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? PaidBrush : UnpaidBrush;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
