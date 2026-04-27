using System.Globalization;

namespace C_971.Converters
{
    public class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isOn && isOn ? "On" : "Off";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return string.Equals(text, "On", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
