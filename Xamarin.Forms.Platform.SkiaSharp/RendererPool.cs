using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public sealed class RendererPool
    {
        readonly Dictionary<Type, Stack<IVisualElementRenderer>> _freeRenderers =
            new Dictionary<Type, Stack<IVisualElementRenderer>>();

        readonly VisualElement _oldElement;

        readonly IVisualElementRenderer _parent;

        public RendererPool(IVisualElementRenderer renderer, VisualElement oldElement)
        {
			_oldElement = oldElement ?? throw new ArgumentNullException(nameof(oldElement));
            _parent = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public IVisualElementRenderer GetFreeRenderer(VisualElement view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var rendererType = Internals.Registrar.Registered.GetHandlerType(view.GetType()) ?? typeof(ViewRenderer);

			if (!_freeRenderers.TryGetValue(rendererType, out Stack<IVisualElementRenderer> renderers) || renderers.Count == 0)
				return null;

			var renderer = renderers.Pop();
            renderer.SetElement(view);
            return renderer;
        }

        public void UpdateNewElement(VisualElement newElement)
        {
            if (newElement == null)
                throw new ArgumentNullException("newElement");

            var sameChildrenTypes = true;

            var oldChildren = _oldElement.LogicalChildren;
            var newChildren = ((IElementController)newElement).LogicalChildren;

            if (oldChildren.Count == newChildren.Count)
            {
                for (var i = 0; i < oldChildren.Count; i++)
                {
                    if (oldChildren[i].GetType() != newChildren[i].GetType())
                    {
                        sameChildrenTypes = false;
                        break;
                    }
                }
            }
            else
                sameChildrenTypes = false;

            if (!sameChildrenTypes)
            {
                ClearRenderers(_parent);
                FillChildrenWithRenderers(newElement);
            }
            else
                UpdateRenderers(newElement);
        }

        void ClearRenderers(IVisualElementRenderer renderer)
        {
            if (renderer == null)
                return;

            var subviews = renderer.Control.Children;
            for (var i = 0; i < subviews.Count; i++)
            {
                var childRenderer = subviews[i] as IVisualElementRenderer;
                if (childRenderer != null)
                {
                    PushRenderer(childRenderer);

                    // The ListView CalculateHeightForCell method can create renderers and dispose its child renderers before this is called.
                    // Thus, it is possible that this work is already completed.
                    if (childRenderer.Element != null && ReferenceEquals(childRenderer, Platform.GetRenderer(childRenderer.Element)))
                        childRenderer.Element.ClearValue(Platform.RendererProperty);
                }
                renderer.Control.RemoveView(subviews[i]);
            }
        }

        void FillChildrenWithRenderers(VisualElement element)
        {
            foreach (var logicalChild in ((IElementController)element).LogicalChildren)
            {
                var child = logicalChild as VisualElement;
                if (child != null)
                {
                    var renderer = GetFreeRenderer(child) ?? Platform.CreateRenderer(child);
                    Platform.SetRenderer(child, renderer);
                    _parent.Control.AddView(renderer.Control);
                }
            }
        }

        void PushRenderer(IVisualElementRenderer renderer)
        {
            var reflectableType = renderer as System.Reflection.IReflectableType;
            var rendererType = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : renderer.GetType();

			if (!_freeRenderers.TryGetValue(rendererType, out Stack<IVisualElementRenderer> renderers))
				_freeRenderers[rendererType] = renderers = new Stack<IVisualElementRenderer>();

			renderers.Push(renderer);
        }

        void UpdateRenderers(Element newElement)
        {
            var newElementController = (IElementController)newElement;

            if (newElementController.LogicalChildren.Count == 0)
                return;

            var subviews = _parent.Control.Children;
            for (var i = 0; i < subviews.Count; i++)
            {
                var childRenderer = subviews[i] as IVisualElementRenderer;
                if (childRenderer == null)
                    continue;

                var element = newElementController.LogicalChildren[i] as VisualElement;

                if (element == null)
                    continue;

                if (childRenderer.Element != null && ReferenceEquals(childRenderer, Platform.GetRenderer(childRenderer.Element)))
                    childRenderer.Element.ClearValue(Platform.RendererProperty);

                childRenderer.SetElement(element);
                Platform.SetRenderer(element, childRenderer);
            }
        }
    }
}