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
    }
}