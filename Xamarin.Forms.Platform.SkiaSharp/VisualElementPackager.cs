using System;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class VisualElementPackager : IDisposable
    {
        private VisualElement _element;
        private bool _isDisposed;

        IElementController ElementController => _element;

        public VisualElementPackager(IVisualElementRenderer renderer) : this(renderer, null)
        {
        }

        VisualElementPackager(IVisualElementRenderer renderer, VisualElement element)
        {
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            renderer.ElementChanged += OnRendererElementChanged;
            SetElement(null, element ?? renderer.Element);
        }

        protected IVisualElementRenderer Renderer { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Load()
        {
            for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
            {
                if (ElementController.LogicalChildren[i] is VisualElement child)
                    OnChildAdded(child);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                SetElement(_element, null);
                if (Renderer != null)
                {
                    Renderer.ElementChanged -= OnRendererElementChanged;
                    Renderer = null;
                }
            }

            _isDisposed = true;
        }

        protected virtual void OnChildAdded(VisualElement view)
        {
            if (_isDisposed)
                return;

            var viewRenderer = Platform.CreateRenderer(view);
            Platform.SetRenderer(view, viewRenderer);

            var nativeView = Renderer.Control;
            nativeView.AddView(viewRenderer.Control);
        }

        protected virtual void OnChildRemoved(VisualElement view)
        {
            if (_element == null)
                return;

            var viewRenderer = Platform.GetRenderer(view);
            if (viewRenderer == null || viewRenderer.Control == null)
                return;

            var parentRenderer = Platform.GetRenderer(_element);
            if (parentRenderer == null || parentRenderer.Control == null)
                return;

            parentRenderer.Control.RemoveView(viewRenderer.Control);
        }

        void OnChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is VisualElement view)
                OnChildAdded(view);
        }

        void OnChildRemoved(object sender, ElementEventArgs e)
        {
            if (e.Element is VisualElement view)
                OnChildRemoved(view);
        }

        void OnRendererElementChanged(object sender, VisualElementChangedEventArgs args)
        {
            if (args.NewElement == _element)
                return;

            SetElement(_element, args.NewElement);
        }

        void SetElement(VisualElement oldElement, VisualElement newElement)
        {
            if (oldElement == newElement)
                return;

            if (oldElement != null)
            {
                oldElement.ChildAdded -= OnChildAdded;
                oldElement.ChildRemoved -= OnChildRemoved;

                if (newElement != null)
                {
                    var pool = new RendererPool(Renderer, oldElement);
                    pool.UpdateNewElement(newElement);
                }
                else
                {
                    var elementController = ((IElementController)oldElement);

                    for (var i = 0; i < elementController.LogicalChildren.Count; i++)
                    {
                        if (elementController.LogicalChildren[i] is VisualElement child)
                            OnChildRemoved(child);
                    }
                }
            }

            _element = newElement;

            if (newElement != null)
            {
                newElement.ChildAdded += OnChildAdded;
                newElement.ChildRemoved += OnChildRemoved;
            }
        }
    }
}