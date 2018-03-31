using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Button : SKView
    {
        public static readonly SKColor DefaultTextColor = SKColors.Black;
        public const float DefaultTextSize = 40.0f;

        private string _text;
        private float _textSize = DefaultTextSize;
        private SKColor _textColor = DefaultTextColor;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
            }
        }

        public float TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;
                Invalidate();
            }
        }

        public SKColor TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
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
                TextSize = TextSize,
                Color = TextColor
            })
            {
                canvas.DrawText(Text, frame.Left, frame.Top + DefaultTextSize, paint);
            }
        }
    }
}