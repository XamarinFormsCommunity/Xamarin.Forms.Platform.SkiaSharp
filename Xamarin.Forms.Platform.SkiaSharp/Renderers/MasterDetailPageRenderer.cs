using SkiaSharp;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class MasterDetailPageRenderer : VisualElementRenderer<MasterDetailPage, Native.MasterDetailPage>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<MasterDetailPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.MasterDetailPage());					
                }
			}
        }
	}
}