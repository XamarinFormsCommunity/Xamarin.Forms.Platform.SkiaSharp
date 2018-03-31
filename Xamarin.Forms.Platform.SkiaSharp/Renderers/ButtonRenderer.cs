using System.ComponentModel;
using Xamarin.Forms.Platform.SkiaSharp.Extensions;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class ButtonRenderer : ViewRenderer<Button, Native.Button>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.Button());
                }

                UpdateText();
                UpdateTextColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.FontProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FontAttributesProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.TextColorProperty.PropertyName)
                UpdateTextColor();
        }

        private void UpdateText()
        {
            Control.Text = Element.Text;
        }

        private void UpdateTextColor()
        {
            Control.TextColor = Element.TextColor.ToSkiaColor();
        }
    }
}