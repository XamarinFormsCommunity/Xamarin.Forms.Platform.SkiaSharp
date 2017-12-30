using System;
using Container = Xamarin.Forms.Platform.SkiaSharp.Controls.SKView;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public interface IVisualElementRenderer : IDisposable, IRegisterable
    {
        event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        VisualElement Element { get; }

        Container NativeView { get; }

        bool Disposed { get; }

        void SetElement(VisualElement element);

        SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

        void SetElementSize(Size size);
    }
}