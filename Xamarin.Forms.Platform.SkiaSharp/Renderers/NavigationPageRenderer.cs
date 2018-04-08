using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
	public class NavigationPageRenderer : VisualElementRenderer<NavigationPage, Native.NavigationPage>
	{
		public NavigationPageRenderer() => SetNativeControl(new Native.NavigationPage());
	}
}