using SkiaSharp;
using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp
{
	public class VisualElementTracker : IDisposable
	{
		bool _disposed;
		VisualElement _element;
		Rectangle _lastBounds;
		Rectangle _lastParentBounds;
		int _updateCount;

		readonly EventHandler<EventArg<VisualElement>> _batchCommittedHandler;
		readonly PropertyChangedEventHandler _propertyChangedHandler;
		readonly EventHandler _sizeChangedEventHandler;

		public event EventHandler NativeControlUpdated;

		public VisualElementTracker(IVisualElementRenderer renderer)
		{
			_propertyChangedHandler = HandlePropertyChanged;
			_sizeChangedEventHandler = HandleSizeChanged;
			_batchCommittedHandler = HandleRedrawNeeded;

			Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
			renderer.ElementChanged += OnRendererElementChanged;
			SetElement(null, renderer.Element);
		}

		void NativeView_Tap(object sender, EventArgs e)
		{
			if (_element is Button button)
				button.SendClicked();
			else if (_element is View view)
				foreach (var recognizer in view.GestureRecognizers)
					if (recognizer is TapGestureRecognizer tapRecognizer)
						tapRecognizer.Command?.Execute(tapRecognizer.CommandParameter);
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
				var nativeView = Renderer.Control;

				if (nativeView != null)
					nativeView.Tap -= NativeView_Tap;

				Renderer.ElementChanged -= OnRendererElementChanged;
				Renderer = null;
			}
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName)
				UpdateNativeControl();
		}

		void HandleSizeChanged(object sender, EventArgs e)
		{
			UpdateNativeControl();
		}

		void HandleRedrawNeeded(object sender, EventArgs e)
		{
			UpdateNativeControl();
		}

		void OnRendererElementChanged(object s, VisualElementChangedEventArgs e)
		{
			if (_element == e.NewElement)
				return;

			SetElement(_element, e.NewElement);
		}

		void SetElement(VisualElement oldElement, VisualElement newElement)
		{
			if (oldElement != null)
			{
				oldElement.PropertyChanged -= _propertyChangedHandler;
				oldElement.SizeChanged -= _sizeChangedEventHandler;
				oldElement.BatchCommitted -= _batchCommittedHandler;
			}

			_element = newElement;

			if (newElement != null)
			{
				newElement.BatchCommitted += _batchCommittedHandler;
				newElement.PropertyChanged += _propertyChangedHandler;
				newElement.SizeChanged += _sizeChangedEventHandler;

				UpdateNativeControl();
			}
		}

		void UpdateNativeControl()
		{
			if (_disposed)
				return;

			OnUpdateNativeControl();

			NativeControlUpdated?.Invoke(this, EventArgs.Empty);
		}

		void OnUpdateNativeControl()
		{
			var view = Renderer.Element;
			var nativeView = Renderer.Control;

			if (view == null || view.Batched)
				return;

			// Attach Gesture Handling
			if (nativeView != null)
			{
				nativeView.Tap -= NativeView_Tap;
				nativeView.Tap += NativeView_Tap;
			}

			var boundsChanged = _lastBounds != view.Bounds;
			var viewParent = view.RealParent as VisualElement;
			var parentBoundsChanged = _lastParentBounds != (viewParent == null ? Rectangle.Zero : viewParent.Bounds);
			var thread = !boundsChanged;

			var width = (float)view.Width;
			var height = (float)view.Height;
			var x = (float)view.X;
			var y = (float)view.Y;

			var updateTarget = Interlocked.Increment(ref _updateCount);

			if (updateTarget != _updateCount)
				return;

			var parent = view.RealParent;

			parentBoundsChanged = true;
			bool shouldUpdate = (width > 0 || height > 0) && parent != null && (boundsChanged || parentBoundsChanged);

			if (shouldUpdate)
			{
				nativeView.Frame = new SKRect(x, y, width + x, height + y); //NOTE: This is odd, I needed to add the y to the height and width to get a property height/width.
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
