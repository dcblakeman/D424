using System.Globalization;

namespace C_971.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isYes && isYes ? "Yes" : "No";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return string.Equals(text, "Yes", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
