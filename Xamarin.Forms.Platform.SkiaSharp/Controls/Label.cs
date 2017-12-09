using SkiaSharp;
using System;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Label : Control, IDisposable
    {
        private string _text;
        private SKColor _textColor;
        private SKTypeface _font;
        private float _fontSize;
        private SKRect? _bounds;
        private SKPoint _location;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
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

        public SKTypeface Font
        {
            get => _font;
            set
            {
                _font = value;
                Invalidate();
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Invalidate();
            }
        }

        public override SKRect Bounds
        {
            get
            {
                if (!_bounds.HasValue)
                {
                    using (var paint = CreatePaint())
                    {
                        var b = new SKRect();
                        paint.MeasureText(Text, ref b);
                        _bounds = SKRect.Create(_location, b.Size);
                    }
                }
                return _bounds.Value;
            }
            set
            {
                _location = value.Location;
                _bounds = value;
    
                Invalidate();
            }
        }

        public void Dispose()
        {
            Font?.Dispose();
            Font = null;
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                using (var paint = CreatePaint())
                {
                    canvas.DrawText(Text, _location.X, _location.Y + Bounds.Height - paint.FontMetrics.Descent, paint);
                }
            }
        }

        private SKPaint CreatePaint()
        {
            return new SKPaint
            {
                IsAntialias = true,
                Color = TextColor,
                Typeface = Font,
                TextSize = FontSize
            };
        }
    }
}