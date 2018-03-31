using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class PageRenderer : VisualElementRenderer<Page, Controls.Page>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Controls.Page());
                }
            }
        }

		public override void Render(SKCanvas canvas)
		{
			// This is obviously just a temporary hack
			// To show a page inside a NavigationPage inside a Detail page of a MasterDetailPage
			// so that we can get the ControlGalleryStarted.

			// This code should be refactored to properly handle Navigation and MasterDetail pages
			// in the future

			if (Element.Parent is NavigationPage navigationPage)
			{
				if (navigationPage.CurrentPage == Element)
					base.Render(canvas);
			}
			else if (!(Element.Parent is MasterDetailPage))
				base.Render(canvas);	
			
		}
	}
}