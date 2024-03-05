using System.Globalization;

namespace Wyrocznia.Converters
{
    public class ColorStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorState = value as string;

            switch (colorState)
            {
                case "Gold":
                    return Colors.Gold;
                case "Green":
                    return Colors.Green;
                case "Red":
                    return Colors.Red;
                default:
                    return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
