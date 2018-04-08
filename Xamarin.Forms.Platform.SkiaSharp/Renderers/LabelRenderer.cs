using System.ComponentModel;
using Xamarin.Forms.Platform.SkiaSharp.Extensions;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class LabelRenderer : ViewRenderer<Label, Native.Label>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Native.Label());
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