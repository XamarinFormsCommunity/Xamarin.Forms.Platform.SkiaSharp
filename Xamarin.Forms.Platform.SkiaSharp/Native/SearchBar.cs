using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Native
{
	public class SearchBar : SKView
	{

		public override SKSize Measure(SKSize available)
		{
			var bounds = new SKRect() { Size = new SKSize(100,50) };
			this.Frame = new SKRect(Frame.Left, Frame.Top, Frame.Left + bounds.Width, Frame.Top + bounds.Height);
			return bounds.Size;
		}
		
		protected override void Render(SKCanvas canvas, SKRect frame)
		{
			base.Render(canvas, frame);

		}
	}
}