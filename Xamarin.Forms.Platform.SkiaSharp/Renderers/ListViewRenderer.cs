using System.ComponentModel;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
	public class ListViewRenderer : ViewRenderer<ListView, Native.ListView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.ListView());
                }
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

        }

    }
}