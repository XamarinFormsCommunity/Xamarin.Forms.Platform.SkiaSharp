using Android.Content;
using Android.Views;
using SkiaSharp;
using SkiaSharp.Views.Android;
using System;
using Xamarin.Forms.Platform.SkiaSharp.Native;

namespace Xamarin.Forms.Platform.SkiaSharp.Android
{
	public class SKViewRenderer : SKCanvasView
    {
        SKView _skView;
        public SKViewRenderer(SKView view, Context context) : base(context)
        {
            _skView = view;
            PaintSurface += OnPaint;
			
            _skView.Invalidated += OnViewInvalidated;
        }

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (e.Action != MotionEventActions.Down)
				return false;

			var data = new TouchData()
			{
				Action = e.Action == MotionEventActions.Down ? TouchAction.Tap : TouchAction.None,
				Point = new Point(e.RawX, e.RawY)
			};

			_skView.HandleTouch(data);

			return data.Handled;
		}

		void OnViewInvalidated(object sender, EventArgs e)
        {			
            _skView.Layout(_skView.Frame);
            _skView.Invalidate();
			this.Invalidate(); // Refresh the whole canvas
        }

        void OnPaint(object sender, SKPaintSurfaceEventArgs e)
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
				OnViewInvalidated(null, null); // Need to trigger a refresh
            }
        }
    }
}