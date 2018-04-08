using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class PageRenderer : VisualElementRenderer<Page, Native.Page>
    {
		public PageRenderer() => SetNativeControl(new Native.Page());

	}
}