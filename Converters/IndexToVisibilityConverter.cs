using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimuladoConcursos.Converters
{
    public class IndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentIndex && parameter is string tipoBotao)
            {
                return tipoBotao switch
                {
                    "Proxima" => currentIndex < 50 ? Visibility.Visible : Visibility.Collapsed,
                    "Finalizar" => currentIndex >= 50 ? Visibility.Visible : Visibility.Collapsed,
                    _ => Visibility.Collapsed
                };
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}