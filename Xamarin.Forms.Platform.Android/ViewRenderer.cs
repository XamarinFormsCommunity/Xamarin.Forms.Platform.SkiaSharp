using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AView = Android.Views.View;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android
{
	public abstract class ViewRenderer : ViewRenderer<View, AView>
	{
		protected ViewRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use ViewRenderer(Context) instead.")]
		protected ViewRenderer()
		{
		}
	}

	public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView>, AView.IOnFocusChangeListener where TView : View where TNativeView : AView
	{
		protected ViewRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use ViewRenderer(Context) instead.")]
		protected ViewRenderer() 
		{
		}

		protected virtual TNativeView CreateNativeControl()
		{
			return default(TNativeView);
		}

		ViewGroup _container;
		string _defaultContentDescription;
		bool? _defaultFocusable;
		string _defaultHint;

		bool _disposed;
		EventHandler<VisualElement.FocusRequestArgs> _focusChangeHandler;

		SoftInput _startingInputMode;

		internal bool HandleKeyboardOnFocus;

		public TNativeView Control { get; private set; }

		void AView.IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			if (Element is Entry || Element is SearchBar || Element is Editor)
			{
				var isInViewCell = false;
				Element parent = Element.RealParent;
				while (!(parent is Page) && parent != null)
				{
					if (parent is Cell)
					{
						isInViewCell = true;
						break;
					}
					parent = parent.RealParent;
				}

				if (isInViewCell)
				{
					Window window = ((Activity)Context).Window;
					if (hasFocus)
					{
						_startingInputMode = window.Attributes.SoftInputMode;
						window.SetSoftInputMode(SoftInput.AdjustPan);
					}
					else
						window.SetSoftInputMode(_startingInputMode);
				}
			}
			OnNativeFocusChanged(hasFocus);

			((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (Control == null)
				return (base.GetDesiredSize(widthConstraint, heightConstraint));

			AView view = _container == this ? (AView)Control : _container;
			view.Measure(widthConstraint, heightConstraint);

			return new SizeRequest(new Size(Control.MeasuredWidth, Control.MeasuredHeight), MinimumSize());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (Control != null && ManageNativeControlLifetime)
				{
					Control.OnFocusChangeListener = null;
					RemoveView(Control);
					Control.Dispose();
					Control = null;
				}

				if (_container != null && _container != this)
				{
					_container.RemoveFromParent();
					_container.Dispose();
					_container = null;
				}

				if (Element != null && _focusChangeHandler != null)
				{
					Element.FocusChangeRequested -= _focusChangeHandler;
					_focusChangeHandler = null;
				}

				_disposed = true;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
		{
			base.OnElementChanged(e);

			if (_focusChangeHandler == null)
				_focusChangeHandler = OnFocusChangeRequested;

			if (e.OldElement != null)
				e.OldElement.FocusChangeRequested -= _focusChangeHandler;

			if (e.NewElement != null)
				e.NewElement.FocusChangeRequested += _focusChangeHandler;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateIsEnabled();
			else if (e.PropertyName == AutomationProperties.LabeledByProperty.PropertyName)
				SetLabeledBy();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
			if (Control == null)
				return;

			AView view = _container == this ? (AView)Control : _container;

			view.Measure(MeasureSpecFactory.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly));
			view.Layout(0, 0, r - l, b - t);
		}

		protected override void OnRegisterEffect(PlatformEffect effect)
		{
			base.OnRegisterEffect(effect);
			effect.SetControl(Control);
		}

		protected override void SetAutomationId(string id)
		{
			if (Control == null)
			{
				base.SetAutomationId(id);
				return;
			}

			ContentDescription = id + "_Container";
			AutomationPropertiesProvider.SetAutomationId(Control, Element, id);
		}

		protected override void SetContentDescription()
		{
			if (Control == null)
			{
				base.SetContentDescription();
				return;
			}

			AutomationPropertiesProvider.SetContentDescription(
				Control, Element, ref _defaultContentDescription, ref _defaultHint);
		}

		protected override void SetFocusable()
		{
			if (Control == null)
			{
				base.SetFocusable();
				return;
			}

			AutomationPropertiesProvider.SetFocusable(Control, Element, ref _defaultFocusable);
		}

		protected void SetNativeControl(TNativeView control)
		{
			SetNativeControl(control, this);
		}

		internal virtual void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			if (Control == null)
				return;

			e.Result = true;

			if (e.Focus)
			{
				// use post being BeginInvokeOnMainThread will not delay on android
				Looper looper = Context.MainLooper;
				var handler = new Handler(looper);
				handler.Post(() =>
				{
					Control?.RequestFocus();
				});
			}
			else
			{
				Control.ClearFocus();
			}

			//handles keyboard on focus for Editor, Entry and SearchBar
			if (HandleKeyboardOnFocus)
			{
				if (e.Focus)
					Control.ShowKeyboard();
				else
					Control.HideKeyboard();
			}
		}

		internal virtual void OnNativeFocusChanged(bool hasFocus)
		{
		}

		internal override void SendVisualElementInitialized(VisualElement element, AView nativeView)
		{
			base.SendVisualElementInitialized(element, Control);
		}

		internal void SetNativeControl(TNativeView control, ViewGroup container)
		{
			if (Control != null)
			{
				Control.OnFocusChangeListener = null;
				RemoveView(Control);
			}

			_container = container;

			Control = control;
			if (Control.Id == NoId)
			{
				Control.Id = Platform.GenerateViewId();
			}

			AView toAdd = container == this ? control : (AView)container;
			AddView(toAdd, LayoutParams.MatchParent);

			Control.OnFocusChangeListener = this;

			UpdateIsEnabled();
			UpdateFlowDirection();
			SetLabeledBy();
		}

		void SetLabeledBy()
			=> AutomationPropertiesProvider.SetLabeledBy(Control, Element);

		void UpdateIsEnabled()
		{
			if (Control != null)
				Control.Enabled = Element.IsEnabled;
		}

		void UpdateFlowDirection()
		{
			Control.UpdateFlowDirection(Element);
		}
	}
}
