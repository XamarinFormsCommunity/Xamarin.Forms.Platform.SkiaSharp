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

            if (e.PropertyName == Button.FontProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Button.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Button.FontAttributesProperty.PropertyName)
                UpdateText();          
            else if (e.PropertyName == Button.TextColorProperty.PropertyName)
                UpdateTextColor();
        }

        void UpdateText()
        {
            Control.Text = Element.Text;
        }

        void UpdateTextColor()
        {
            Control.TextColor = Element.TextColor.ToSkiaColor();
        }
    }
}