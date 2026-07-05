using System.Windows.Media;

namespace SpectrumVisualizer.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color from, Color to, double t)
        {
            byte r = (byte)(from.R + (to.R - from.R) * t);
            byte g = (byte)(from.G + (to.G - from.G) * t);
            byte b = (byte)(from.B + (to.B - from.B) * t);
            return Color.FromRgb(r, g, b);
        }
    }
}
