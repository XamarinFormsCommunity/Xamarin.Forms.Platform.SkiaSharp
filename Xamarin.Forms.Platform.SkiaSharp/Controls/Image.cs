using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Image : Control
    {
        private SKBitmap _source;

        public SKBitmap Source
        {
            get => _source;
            set
            {
                _source = value;
                Invalidate();
            }
        }

        public override void Draw(SKCanvas canvas)
        {
            if (_source != null)
            {
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    if (_source != null)
                    {
                        DrawBitmap(canvas, paint);
                    }
                }
            }
        }

        private void DrawBitmap(SKCanvas canvas, SKPaint paint)
        {
            var bitmapRect = GetBitmapRect();

            canvas.DrawBitmap(_source, bitmapRect, Bounds, paint);
        }

        private SKRect GetBitmapRect()
        {
            return SKRect.Create(0, 0, _source.Width, _source.Height);
        }
    }
}