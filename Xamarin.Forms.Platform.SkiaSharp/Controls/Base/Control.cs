using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class Control 
    {
        private SKRect _bounds;

        public virtual SKRect Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                Invalidate();
            }
        }

        public SKPoint Location
        {
            get => Bounds.Location;
            set { Bounds = SKRect.Create(value, Size); }
        }

        public float X
        {
            get => Location.X;
            set { Location = new SKPoint(value, Y); }
        }

        public float Y
        {
            get => Location.Y;
            set { Location = new SKPoint(X, value); }
        }

        public SKSize Size
        {
            get => Bounds.Size;
            set { Bounds = SKRect.Create(Location, value); }
        }

        public float Width
        {
            get => Size.Width;
            set { Size = new SKSize(value, Height); }
        }

        public float Height
        {
            get => Size.Height;
            set { Size = new SKSize(Width, value); }
        }

        internal ControlManager Parent { get; set; }

        public void Invalidate()
        {
            Parent?.Invalidate();
        }

        public virtual void Draw(SKCanvas canvas)
        {

        }
    }
}