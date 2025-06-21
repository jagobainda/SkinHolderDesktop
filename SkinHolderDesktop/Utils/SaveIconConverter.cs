using FontAwesome.WPF;
using System.Globalization;
using System.Windows.Data;

namespace SkinHolderDesktop.Utils;

public class SaveIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is true) ? FontAwesomeIcon.Save : FontAwesomeIcon.Ban;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
