using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class SliderRenderer : ViewRenderer<Slider, SeekBar>, SeekBar.IOnSeekBarChangeListener
	{
		double _max, _min;
		bool _isTrackingChange;
		ColorStateList defaultprogresstintlist, defaultprogressbackgroundtintlist;
		ColorFilter defaultthumbcolorfilter;
		Drawable defaultthumb;
		PorterDuff.Mode defaultprogresstintmode, defaultprogressbackgroundtintmode;

		public SliderRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use SliderRenderer(Context) instead.")]
		public SliderRenderer()
		{
			AutoPackage = false;
		}

		double Value
		{
			get { return _min + (_max - _min) * (Control.Progress / 1000.0); }
			set { Control.Progress = (int)((value - _min) / (_max - _min) * 1000.0); }
		}

		void SeekBar.IOnSeekBarChangeListener.OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (_isTrackingChange)
				((IElementController)Element).SetValueFromRenderer(Slider.ValueProperty, Value);
		}

		void SeekBar.IOnSeekBarChangeListener.OnStartTrackingTouch(SeekBar seekBar)
		{
			_isTrackingChange = true;
		}

		void SeekBar.IOnSeekBarChangeListener.OnStopTrackingTouch(SeekBar seekBar)
		{
			_isTrackingChange = false;
		}

		protected override SeekBar CreateNativeControl()
		{
			return new FormsSeekBar(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var seekBar = CreateNativeControl();
				SetNativeControl(seekBar);

				seekBar.Max = 1000;
				seekBar.SetOnSeekBarChangeListener(this);

				if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
				{
					defaultthumbcolorfilter = seekBar.Thumb.ColorFilter;
					defaultprogresstintmode = seekBar.ProgressTintMode;
					defaultprogressbackgroundtintmode = seekBar.ProgressBackgroundTintMode;
					defaultprogresstintlist = seekBar.ProgressTintList;
					defaultprogressbackgroundtintlist = seekBar.ProgressBackgroundTintList;
					defaultthumb = seekBar.Thumb;
				}
			}

			Slider slider = e.NewElement;
			_min = slider.Minimum;
			_max = slider.Maximum;
			Value = slider.Value;
			if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
			{
				UpdateSliderColors();
			}
		}

		SeekBar NativeSeekbar
		{
			get { return Control; }
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			Slider view = Element;
			switch (e.PropertyName)
			{
				case "Maximum":
					_max = view.Maximum;
					break;
				case "Minimum":
					_min = view.Minimum;
					break;
				case "Value":
					if (Value != view.Value)
						Value = view.Value;
					break;
			}

			if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
			{
				if (e.PropertyName == Slider.MinimumTrackColorProperty.PropertyName)
					UpdateMinimumTrackColor();
				else if (e.PropertyName == Slider.MaximumTrackColorProperty.PropertyName)
					UpdateMaximumTrackColor();
				else if (e.PropertyName == Slider.ThumbImageProperty.PropertyName)
					UpdateThumbImage();
				else if (e.PropertyName == Slider.ThumbColorProperty.PropertyName)
					UpdateThumbColor();
			}
		}

		private void UpdateSliderColors()
		{
			UpdateMinimumTrackColor();
			UpdateMaximumTrackColor();
			if (!string.IsNullOrEmpty(Element.ThumbImage))
			{
				UpdateThumbImage();
			}
			else
			{
				UpdateThumbColor();
			}
		}

		private void UpdateMinimumTrackColor()
		{
			if (Element != null)
			{
				if (Element.MinimumTrackColor == Color.Default)
				{
					Control.ProgressTintList = defaultprogresstintlist;
					Control.ProgressTintMode = defaultprogresstintmode;
				}
				else
				{
					Control.ProgressTintList = ColorStateList.ValueOf(Element.MinimumTrackColor.ToAndroid());
					Control.ProgressTintMode = PorterDuff.Mode.SrcIn;
				}
			}
		}

		private void UpdateMaximumTrackColor()
		{
			if (Element != null)
			{
				if (Element.MaximumTrackColor == Color.Default)
				{
					Control.ProgressBackgroundTintList = defaultprogressbackgroundtintlist;
					Control.ProgressBackgroundTintMode = defaultprogressbackgroundtintmode;
				}
				else
				{
					Control.ProgressBackgroundTintList = ColorStateList.ValueOf(Element.MaximumTrackColor.ToAndroid());
					Control.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
				}
			}
		}

		private void UpdateThumbColor()
		{
			if (Element != null)
			{
				if (Element.ThumbColor == Color.Default)
				{
					Control.Thumb.SetColorFilter(defaultthumbcolorfilter);
				}
				else
				{
					Control.Thumb.SetColorFilter(Element.ThumbColor.ToAndroid(), PorterDuff.Mode.SrcIn);
				}

			}
		}

		private void UpdateThumbImage()
		{
			if (Element != null)
			{
				if (string.IsNullOrEmpty(Element.ThumbImage))
					Control.SetThumb(defaultthumb);
				else
					Control.SetThumb(Context.GetDrawable(Element.ThumbImage));
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			BuildVersionCodes androidVersion = Build.VERSION.SdkInt;
			if (androidVersion < BuildVersionCodes.JellyBean)
				return;

			// Thumb only supported JellyBean and higher

			if (Control == null)
				return;

			SeekBar seekbar = Control;

			Drawable thumb = seekbar.Thumb;
			int thumbTop = seekbar.Height / 2 - thumb.IntrinsicHeight / 2;

			thumb.SetBounds(thumb.Bounds.Left, thumbTop, thumb.Bounds.Left + thumb.IntrinsicWidth, thumbTop + thumb.IntrinsicHeight);
		}
	}
}
