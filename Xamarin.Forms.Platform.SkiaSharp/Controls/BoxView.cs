using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class BoxView : SKView
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

        protected override void Render(SKCanvas canvas, SKRect frame)
        {
            base.Render(canvas, frame);

            using (var paint = new SKPaint { IsAntialias = true })
            {
                paint.Color = Color;
                canvas.DrawRect(frame, paint);
            }
        }
    }
}