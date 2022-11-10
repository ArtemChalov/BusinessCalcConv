using System.Globalization;

namespace BusinessCalculator.Converters;

public class FileNameToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ImageSource.FromFile(value?.ToString() ?? "empty.png");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
