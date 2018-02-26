using System.Windows.Media;

namespace GestionFormation.App.Views.EditableLists.Formations
{
    public static class ColorHelper
    {
        public static Color FromInt(int color)
        {
            return Color.FromArgb((byte)(color >> 24),
                (byte)(color >> 16),
                (byte)(color >> 8),
                (byte)(color));
        }

        public static int ToInt(Color color)
        {
            var iCol = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
            return iCol;
        }
    }
}