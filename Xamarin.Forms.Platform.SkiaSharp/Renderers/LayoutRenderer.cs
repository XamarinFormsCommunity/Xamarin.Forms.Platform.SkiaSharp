using Panel = Xamarin.Forms.Platform.SkiaSharp.Controls.SKView;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class LayoutRenderer : ViewRenderer<Layout, Panel>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Panel());
                }
            }
        }
    }
}