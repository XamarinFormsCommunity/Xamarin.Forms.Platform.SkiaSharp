using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Native
{
    public class Button : SKView
    {
        public static readonly SKColor DefaultTextColor = SKColors.Black;
        public const float DefaultTextSize = 40.0f;

        string _text;
        float _textSize = DefaultTextSize;
        SKColor _textColor = DefaultTextColor;

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
		
		public override SKSize Measure(SKSize available)
		{
			using (var paint = new SKPaint
			{
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				TextSize = TextSize,
				Color = TextColor
			})
			{
				var bounds = new SKRect();
				paint.MeasureText(Text, ref bounds);
				this.Frame = new SKRect(Frame.Left, Frame.Top, Frame.Left + bounds.Width, Frame.Top + bounds.Height);
				return new SKSize(bounds.Width, bounds.Height);
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