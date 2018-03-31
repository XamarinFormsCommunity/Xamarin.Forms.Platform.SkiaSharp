using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
	public class NavigationPageRenderer : VisualElementRenderer<NavigationPage, Controls.NavigationPage>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new Controls.NavigationPage());
				}
			}
		}		
	}
}