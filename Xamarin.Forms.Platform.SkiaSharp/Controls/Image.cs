using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Image : SKView
    {
        private SKBitmap _source;

        public Image()
        {
            _source = new SKBitmap();
        }

        public SKBitmap Source
        {
            get => _source;
            set
            {
                _source = value;
                Invalidate();
            }
        }

        protected override void Render(SKCanvas canvas, SKRect frame)
        {
            base.Render(canvas, frame);

            using (var paint = new SKPaint { IsAntialias = true })
            {
                canvas.DrawBitmap(Source, frame.Left, frame.Top, paint);
            }
        }
    }
}