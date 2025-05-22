using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SimuladoConcursos.ViewModels;

namespace SimuladoConcursos.Converters
{
    public class IndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentIndex && parameter is Visibility lastPageVisibility)
            {
                var vm = Application.Current.MainWindow?.DataContext as MainViewModel;
                if (vm != null && currentIndex == vm.Questions.Count - 1)
                {
                    return lastPageVisibility;
                }
                return lastPageVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}