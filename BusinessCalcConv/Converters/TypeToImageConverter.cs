using System.Globalization;

namespace BusinessCalculator.Converters
{
    public sealed class TypeToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;

            return value.GetType().Name switch
            {
                "AngleConverter" => ImageSource.FromFile("angle.png"),
                "AreaConverter" => ImageSource.FromFile("area.png"),
                "DataStoreConverter" => ImageSource.FromFile("storage.png"),
                "DataTransferConverter" => ImageSource.FromFile("datatransfer.png"),
                "EnergyConverter" => ImageSource.FromFile("energy.png"),
                "ForceConverter" => ImageSource.FromFile("force.png"),
                "LengthConverter" => ImageSource.FromFile("length.png"),
                "LightConverter" => ImageSource.FromFile("lighting.png"),
                "NDSConverter" => ImageSource.FromFile("mony.png"),
                "PowerConverter" => ImageSource.FromFile("power.png"),
                "PressureConverter" => ImageSource.FromFile("pressure.png"),
                "SpeedConverter" => ImageSource.FromFile("speed.png"),
                "TemperatureConverter" => ImageSource.FromFile("temperature.png"),
                "TimeConverter" => ImageSource.FromFile("time.png"),
                "VolumeConverter" => ImageSource.FromFile("volume.png"),
                "WeightConverter" => ImageSource.FromFile("weight.png"),
                _ => ImageSource.FromFile("empty.png")
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
