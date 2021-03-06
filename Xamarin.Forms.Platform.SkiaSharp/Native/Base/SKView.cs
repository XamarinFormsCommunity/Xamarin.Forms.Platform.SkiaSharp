﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.SkiaSharp.Native
{
    public class SKView
    {
        public const int InvalidateThrottle = 10;

        bool _isInvalidated;
        SKColor _backgroundColor;
        SKRect _frame;
        SKView _parent;
        List<SKView> _children = new List<SKView>();

		public event EventHandler Tap;
        public event EventHandler Invalidated;

        public SKView Parent => _parent;

        public SKRect AbsoluteFrame => Parent == null ? 
            Frame : SKRect.Create(
            Parent.Frame.Left + Frame.Left,
            Parent.Frame.Top + Frame.Top,
            Parent.Frame.Size.Width,
            Parent.Frame.Size.Height);
        
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
            set
            {
                _backgroundColor = value;
                Invalidate();
            }
        }

		public void HandleTouch(TouchData touch)
		{
			if (!(this is Native.Label) && this.Frame.Contains((float)touch.Point.X, (float)touch.Point.Y))
				foreach (var child in Children)
					child.HandleTouch(touch);
			else
				Tap?.Invoke(this, new EventArgs());
		}

        public List<SKView> Children => _children;

		public virtual SKSize Measure(SKSize available) => available;

        public virtual void Render(SKCanvas canvas)
        {
            var absolute = AbsoluteFrame;

            Render(canvas, absolute);

            foreach (var child in _children)
            {
                child.Render(canvas);
            }
        }

        public virtual void Layout(SKRect frame) {
			
		}

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

                await Task.Delay(InvalidateThrottle);

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
    }
}