using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Label : SKView
    {
        public static readonly SKColor DefaultTextColorh = SKColors.Black;
        public const float DefaultTextSize = 40.0f;

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
            }
        }

        protected override void Render(SKCanvas canvas, SKRect frame)
        {
            base.Render(canvas, frame);

            using (var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextSize = DefaultTextSize,
                Color = DefaultTextColorh
            })
            {
                canvas.DrawText(Text, frame.Left, frame.Top + DefaultTextSize, paint);
            }
        }
    }
}