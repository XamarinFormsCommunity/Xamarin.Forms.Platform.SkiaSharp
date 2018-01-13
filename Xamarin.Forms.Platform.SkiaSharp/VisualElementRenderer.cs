using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.Platform.SkiaSharp.Extensions;
using Container = Xamarin.Forms.Platform.SkiaSharp.Controls.SKView;
using Control = Xamarin.Forms.Platform.SkiaSharp.Controls.SKView;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class VisualElementRenderer<TElement, TNativeElement> : Container, IVisualElementRenderer, IDisposable
      where TElement : VisualElement
      where TNativeElement : Control
    {
        private bool _disposed;
        private VisualElementPackager _packager;
        private VisualElementTracker _tracker;

        private readonly PropertyChangedEventHandler _propertyChangedHandler;
        private readonly List<EventHandler<VisualElementChangedEventArgs>> _elementChangedHandlers = new List<EventHandler<VisualElementChangedEventArgs>>();

        protected VisualElementRenderer()
        {
            _propertyChangedHandler = OnElementPropertyChanged;
        }

        event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
        {
            add { _elementChangedHandlers.Add(value); }
            remove { _elementChangedHandlers.Remove(value); }
        }

        public TNativeElement Control { get; set; }

        public TElement Element { get; set; }

        public Container NativeView => this;

        public bool Disposed { get { return _disposed; } }

        VisualElement IVisualElementRenderer.Element
        {
            get
            {
                return Element;
            }
        }

        protected IElementController ElementController => Element as IElementController;

        public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;

        public void Dispose()
        {
            _disposed = true;
        }

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (Control == null)
                return new SizeRequest();

            var constraint = new SKSize((float)widthConstraint, (float)heightConstraint);

            Control.Measure(constraint);

            return new SizeRequest(new Size(Math.Ceiling(Control.Frame.Width), Math.Ceiling(Control.Frame.Height)));
        }

        void IVisualElementRenderer.SetElement(VisualElement element)
        {
            SetElement((TElement)element);
        }

        public void SetElement(TElement element)
        {
            var oldElement = Element;
            Element = element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= _propertyChangedHandler;
            }

            if (element != null)
            {
                if (_tracker == null)
                {
                    _tracker = new VisualElementTracker(this);
                    _tracker.NativeControlUpdated += (sender, e) => UpdateNativeControl();
                }

                if (_packager == null)
                {
                    _packager = new VisualElementPackager(this);
                    _packager.Load();
                }

                element.PropertyChanged += _propertyChangedHandler;
            }

            OnElementChanged(new ElementChangedEventArgs<TElement>(oldElement, element));
        }

        public void SetElementSize(Size size)
        {
            Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(Element,
                new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        }

        protected virtual void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            var args = new VisualElementChangedEventArgs(e.OldElement, e.NewElement);
            for (var i = 0; i < _elementChangedHandlers.Count; i++)
                _elementChangedHandlers[i](this, args);

            ElementChanged?.Invoke(this, e);
        }

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
                UpdateIsVisible();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }

        protected virtual void SetNativeControl(TNativeElement view)
        {
            Control = view;

            UpdateBackgroundColor();
            UpdateIsVisible();
        }
        protected virtual void UpdateNativeControl()
        {

        }

        private void UpdateIsVisible()
        {
            if (_disposed || Element == null || Control == null)
                return;

            var isVisible = Element.IsVisible;
        }

        private void UpdateBackgroundColor()
        {
            if (_disposed || Element == null || Control == null)
                return;

            Color backgroundColor = Element.BackgroundColor;

            bool isDefault = backgroundColor.IsDefaultOrTransparent();

            if (!isDefault)
            {
                var color = backgroundColor.ToSkiaColor();

                Control.BackgroundColor = color;
            }
        }
    }
}