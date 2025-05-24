using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SimuladoConcursos.Converters
{
    public class ScoreToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int score)
            {
                return score switch
                {
                    >= 800 => new SolidColorBrush(Colors.Green),
                    >= 600 => new SolidColorBrush(Colors.Orange),
                    _ => new SolidColorBrush(Colors.Red)
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}