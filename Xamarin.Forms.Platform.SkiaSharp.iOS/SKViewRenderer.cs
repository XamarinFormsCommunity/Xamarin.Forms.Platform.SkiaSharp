using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using Xamarin.Forms.Platform.SkiaSharp.Controls;

namespace Xamarin.Forms.Platform.SkiaSharp.iOS
{
    public class SKViewRenderer : SKCanvasView
    {
        private SKView _skView;

        public SKViewRenderer(SKView view)
        {
            _skView = view;
            PaintSurface += OnPaint;
            view.Invalidated += OnViewInvalidated;
        }

        private void OnViewInvalidated(object sender, EventArgs e)
        {
            _skView.Layout(_skView.Frame);
            SetNeedsDisplayInRect(Bounds);
        }

        private void OnPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.White);
            _skView.Render(e.Surface.Canvas);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _skView.Frame = SKRect.Create(SKPoint.Empty, ToPlatform(new SKSize((float)Bounds.Size.Width, (float)Bounds.Size.Height)));
        }

        private float Density => 2;

        private SKPoint FromPlatform(SKPoint point)
        {
            return new SKPoint(point.X / Density, point.Y / Density);
        }

        private SKSize FromPlatform(SKSize point)
        {
            return new SKSize(point.Width / Density, point.Height / Density);
        }

        private SKPoint ToPlatform(SKPoint point)
        {
            return new SKPoint(point.X * Density, point.Y * Density);
        }

        private SKSize ToPlatform(SKSize point)
        {
            return new SKSize(point.Width * Density, point.Height * Density);
        }
    }
}
