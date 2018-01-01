using SkiaSharp;
using System;
using System.ComponentModel;
using System.Threading;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class VisualElementTracker : IDisposable
    {
        private bool _disposed;
        private VisualElement _element;
        private Rectangle _lastBounds;
        private Rectangle _lastParentBounds;
        private int _updateCount;

        readonly PropertyChangedEventHandler _propertyChangedHandler;
        readonly EventHandler _sizeChangedEventHandler;

        public VisualElementTracker(IVisualElementRenderer renderer)
        {
            _propertyChangedHandler = HandlePropertyChanged;
            _sizeChangedEventHandler = HandleSizeChanged;

            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            renderer.ElementChanged += OnRendererElementChanged;
            SetElement(null, renderer.Element);
        }

        IVisualElementRenderer Renderer { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                SetElement(_element, null);

                Renderer.ElementChanged -= OnRendererElementChanged;
                Renderer = null;
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                e.PropertyName == VisualElement.HeightProperty.PropertyName || e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName ||
                e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName || e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
                e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
                e.PropertyName == VisualElement.IsVisibleProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == VisualElement.InputTransparentProperty.PropertyName || e.PropertyName == VisualElement.OpacityProperty.PropertyName)
                UpdateNativeControl();
        }

        private void HandleSizeChanged(object sender, EventArgs e)
        {
            UpdateNativeControl();
        }

        private void OnRendererElementChanged(object s, VisualElementChangedEventArgs e)
        {
            if (_element == e.NewElement)
                return;

            SetElement(_element, e.NewElement);
        }

        private void SetElement(VisualElement oldElement, VisualElement newElement)
        {
            if (oldElement != null)
            {
                oldElement.PropertyChanged -= _propertyChangedHandler;
                oldElement.SizeChanged -= _sizeChangedEventHandler;
            }

            _element = newElement;

            if (newElement != null)
            {
                newElement.PropertyChanged += _propertyChangedHandler;
                newElement.SizeChanged += _sizeChangedEventHandler;

                UpdateNativeControl();
            }
        }

        private void UpdateNativeControl()
        {
            if (_disposed)
                return;

            OnUpdateNativeControl();
        }

        private void OnUpdateNativeControl()
        {
            var view = Renderer.Element;
            var nativeView = Renderer.NativeView;

            if (view == null || view.Batched)
                return;

            var boundsChanged = _lastBounds != view.Bounds;
            var viewParent = view.RealParent as VisualElement;
            var parentBoundsChanged = _lastParentBounds != (viewParent == null ? Rectangle.Zero : viewParent.Bounds);
            var thread = !boundsChanged;

            var anchorX = (float)view.AnchorX;
            var anchorY = (float)view.AnchorY;
            var translationX = (float)view.TranslationX;
            var translationY = (float)view.TranslationY;
            var rotationX = (float)view.RotationX;
            var rotationY = (float)view.RotationY;
            var rotation = (float)view.Rotation;
            var scale = (float)view.Scale;
            var width = (float)view.Width;
            var height = (float)view.Height;
            var x = (float)view.X;
            var y = (float)view.Y;

            var updateTarget = Interlocked.Increment(ref _updateCount);

            if (updateTarget != _updateCount)
                return;

            var parent = view.RealParent;

            parentBoundsChanged = true;
            bool shouldUpdate = width > 0 && height > 0 && parent != null && (boundsChanged || parentBoundsChanged);

            if (shouldUpdate)
            {
                nativeView.Frame = new SKRect(x, y, width, height);
            }
            else if (width <= 0 || height <= 0)
            {
                return;
            }

            _lastBounds = view.Bounds;
            _lastParentBounds = viewParent?.Bounds ?? Rectangle.Zero;
        }
    }
}
