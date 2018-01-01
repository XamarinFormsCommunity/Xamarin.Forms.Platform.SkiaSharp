using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class SKView
    {
        public const int InvalidateTrottle = 10;

        private bool _isInvalidated;
        private SKColor _backgroundColor;
        private SKRect _frame;
        private SKView _parent;
        private List<SKView> _children = new List<SKView>();

        public event EventHandler Invalidated;

        public SKView Parent => _parent;

        public SKRect AbsoluteFrame => Parent == null ? Frame : SKRect.Create(
            Parent.Frame.Left + Frame.Left,
            Parent.Frame.Top + Frame.Top,
            Frame.Size.Width,
            Frame.Size.Height);
        
        public SKRect Frame
        {
            get => _frame;
            set
            {
                if (_frame != value)
                {
                    _frame = value;
                    Layout(value);
                    Invalidate();
                }
            }
        }

        public SKColor BackgroundColor
        {
            get => _backgroundColor;
            set => SetAndInvalidate(ref _backgroundColor, value);
        }

        public List<SKView> Children => _children;

        public virtual SKSize Measure(SKSize available) => available;

        public void Render(SKCanvas canvas)
        {
            var absolute = AbsoluteFrame;

            Render(canvas, absolute);

            foreach (var child in _children)
            {
                child.Render(canvas);
            }
        }

        public virtual void Layout(SKRect frame) { }

        protected virtual void Render(SKCanvas canvas, SKRect frame)
        {
            if (BackgroundColor != null)
            {
                using (var paint = new SKPaint()
                {
                    Color = BackgroundColor,
                    Style = SKPaintStyle.Fill
                })
                {
                    canvas.DrawRect(frame, paint);
                }
            }
        }

        public async void Invalidate()
        {
            if (!_isInvalidated)
            {
                _isInvalidated = true;
                await Task.Delay(InvalidateTrottle);

                Invalidated?.Invoke(this, EventArgs.Empty);

                var root = Parent;
                while (root?.Parent != null)
                    root = root.Parent;

                if (root != null && root != this)
                    root.Invalidate();

                _isInvalidated = false;
            }
        }

        public void AddViews(params SKView[] children)
        {
            foreach (var child in children)
            {
                AddView(child);
            }
        }

        public void AddView(SKView child)
        {
            if (child.Parent != null)
                throw new InvalidOperationException("Added view already has a parent");

            child._parent = this;
            _children.Add(child);
            Invalidate();
        }

        public void RemoveView(SKView child)
        {
            if (child.Parent != this)
                throw new InvalidOperationException("Removed view isn't a child of this view");

            child._parent = null;
            _children.Remove(child);
            Invalidate();
        }

        protected bool SetAndInvalidate<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                Invalidate();

                return true;
            }

            return false;
        }
    }
}