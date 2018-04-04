using System.ComponentModel;
using Xamarin.Forms.Platform.SkiaSharp.Extensions;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class BoxViewRenderer : ViewRenderer<BoxView, Native.BoxView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.BoxView());
                }

                UpdateColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BoxView.ColorProperty.PropertyName)
                UpdateColor();
        }

        private void UpdateColor()
        {
            Control.Color = Element.Color.ToSkiaColor();
        }
    }
}