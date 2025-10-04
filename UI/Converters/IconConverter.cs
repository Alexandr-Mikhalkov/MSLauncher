using System.Windows.Data;
using System.Globalization;
using MSLauncher.UI.Utils;

namespace MSLauncher.UI.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var icon = (MessageIcon)value;
            return icon switch
            {
                MessageIcon.Warning => "/Resources/warning.png",
                MessageIcon.Error => "/Resources/error.png",
                _ => "/Resources/info.png"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}