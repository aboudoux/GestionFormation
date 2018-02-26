using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace GestionFormation.App.Views.EditableLists.Formations
{
    public class IntToColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var SomeInt = (int)value;
            return IntToColor(SomeInt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var col = (Color)value;
            return ColorToInt(col);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        int ColorToInt(Color color)
        {
            return ColorHelper.ToInt(color);
        }

        Color IntToColor(int iCol)
        {
            return ColorHelper.FromInt(iCol);
        }
    }
}