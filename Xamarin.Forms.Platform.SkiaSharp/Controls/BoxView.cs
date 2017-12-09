using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class BoxView : Control
    {
        private SKColor _color;

        public SKColor Color
        {
            get => _color;
            set
            {
                _color = value;
                Invalidate();
            }
        }

        public override void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint { IsAntialias = true })
            {
                paint.Color = Color;
                canvas.DrawRect(Bounds, paint);
            }
        }
    }
}