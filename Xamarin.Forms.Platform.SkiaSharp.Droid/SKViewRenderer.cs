using System;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Xamarin.Forms.Platform.SkiaSharp.Controls;

namespace Xamarin.Forms.Platform.SkiaSharp.Droid
{
    public class SKViewRenderer : SKCanvasView
    {
        private SKView _skView;

        public SKViewRenderer(SKView view, Context context) : base(context)
        {
            _skView = view;
            PaintSurface += OnPaint;
            _skView.Invalidated += OnViewInvalidated; 
        }

        private void OnViewInvalidated(object sender, EventArgs e)
        {
            _skView.Layout(_skView.Frame);
            _skView.Invalidate();
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);
            _skView.Render(canvas);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (w != oldw || h != oldh)
            {
                _skView.Frame = SKRect.Create(SKPoint.Empty, new SKSize(w, h));
            }
        }
    }
}