using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Extensions
{
    public static class ColorExtensions
    {
        public static SKColor ToSkiaColor(this Color color)
        {
            string hex = color.ToRgbaColor();
            var skiaColor = SKColor.Parse(hex);

            return skiaColor;
        }

        internal static string ToRgbaColor(this Color color)
        {
            int red = (int)(color.R * 255);
            int green = (int)(color.G * 255);
            int blue = (int)(color.B * 255);

            return string.Format("#{0:X2}{1:X2}{2:X2}", red, green, blue);
        }

        internal static bool IsDefaultOrTransparent(this Color color)
        {
            return color == Color.Transparent || color == Color.Default;
        }
    }
}