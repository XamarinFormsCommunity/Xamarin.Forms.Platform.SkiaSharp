﻿using Android.Content;
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
			this.Touch += (s, e) =>
			{
				if (e.Event.Action != global::Android.Views.MotionEventActions.Up)
					return;

				_skView.HandleTouch(new TouchData()
				{
					Action = e.Event.Action == global::Android.Views.MotionEventActions.Up ? TouchAction.Tap : TouchAction.None,					
					Point= new Point(e.Event.RawX, e.Event.RawY)
				});
			};
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