using System.ComponentModel;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class LabelRenderer : ViewRenderer<Label, Controls.Label>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Controls.Label());
                }

                UpdateText();
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
        }

        private void UpdateText()
        {
            Control.Text = Element.Text;
        }
    }
}