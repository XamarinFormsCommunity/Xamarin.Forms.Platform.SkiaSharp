using System.ComponentModel;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
	public class SearchBarRenderer : ViewRenderer<SearchBar, Native.SearchBar>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.SearchBar());
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