using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms.Platform.SkiaSharp.Native;

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
			MultipleTouchEnabled = true;
			this.UserInteractionEnabled = true;
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			foreach (UITouch touch in touches.Cast<UITouch>())
			{
				TouchEvent(touch);				
			}
		}

		void TouchEvent(UITouch touch)
		{
			var originPoint = touch.LocationInView(this);
			
			var scaledPoint = new Point(originPoint.X * Density, originPoint.Y * Density);

			var point = new SKPoint((float)(scaledPoint.X), (float)(scaledPoint.Y));

			var data = new TouchData()
			{
				Action = TouchAction.Tap,
				Point = new Point(point.X, point.Y)
			};

			_skView.HandleTouch(data);
		}

		void OnViewInvalidated(object sender, EventArgs e)
		{
			_skView.Layout(_skView.Frame);
			SetNeedsDisplayInRect(Bounds);
		}

		void OnPaint(object sender, SKPaintSurfaceEventArgs e)
		{
			e.Surface.Canvas.Clear(SKColors.White);
			_skView.Render(e.Surface.Canvas);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			_skView.Frame = SKRect.Create(SKPoint.Empty, ToPlatform(new SKSize((float)Bounds.Size.Width, (float)Bounds.Size.Height)));
		}

		float Density => 2;

		SKPoint FromPlatform(SKPoint point)
		{
			return new SKPoint(point.X / Density, point.Y / Density);
		}

		SKSize FromPlatform(SKSize point)
		{
			return new SKSize(point.Width / Density, point.Height / Density);
		}

		SKPoint ToPlatform(SKPoint point)
		{
			return new SKPoint(point.X * Density, point.Y * Density);
		}

		 SKSize ToPlatform(SKSize point)
		{
			return new SKSize(point.Width * Density, point.Height * Density);
		}
	}
}
