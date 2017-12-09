using System;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public interface IVisualElementRenderer : IDisposable, IRegisterable
    {
        event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        VisualElement Element { get; }

        Controls.Control Container { get; }

        bool Disposed { get; }

        void SetElement(VisualElement element);

        SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

        void SetElementSize(Size size);
    }
}