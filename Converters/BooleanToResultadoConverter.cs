using System;
using System.Globalization;
using System.Windows.Data;

namespace SimuladoConcursos.Converters
{
    public class BooleanToResultadoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? "Acertou" : "Errou";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}