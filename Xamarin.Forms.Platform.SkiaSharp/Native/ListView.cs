using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Native
{
	public class ListView : SKView
	{

		public override SKSize Measure(SKSize available)
		{
			return new SKSize(available.Width, 50);
		}
		
		protected override void Render(SKCanvas canvas, SKRect frame)
		{
			base.Render(canvas, frame);

		}
	}
}