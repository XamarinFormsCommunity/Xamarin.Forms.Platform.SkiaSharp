﻿using System;
using Container = Xamarin.Forms.Platform.SkiaSharp.Native.SKView;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public interface IVisualElementRenderer : IDisposable, IRegisterable
    {
        event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        VisualElement Element { get; }

        Container Control { get; }

        bool Disposed { get; }

        void SetElement(VisualElement element);

        SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

        void SetElementSize(Size size);
    }
}