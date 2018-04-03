﻿using System;
using System.ComponentModel;
using UIKit;
using PlatformElement = Xamarin.Forms.PlatformConfiguration.iOSSpecific.VisualElement;


namespace Xamarin.Forms.Platform.iOS
{
	internal class ShadowEffect : PlatformEffect
	{
		UIView ShadowView => Control ?? Container;

		protected override void OnAttached() => UpdateShadow();

		protected override void OnDetached()
		{
			var layer = ShadowView.Layer;
			layer.ShadowColor = Color.Transparent.ToCGColor();
			layer.ShadowOpacity = 0;
		}

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			if (args.PropertyName == PlatformElement.IsShadowEnabledProperty.PropertyName ||
				args.PropertyName == PlatformElement.ShadowColorProperty.PropertyName ||
				args.PropertyName == PlatformElement.ShadowOffsetProperty.PropertyName ||
				args.PropertyName == PlatformElement.ShadowRadiusProperty.PropertyName ||
				args.PropertyName == PlatformElement.ShadowOpacityProperty.PropertyName)
			{
				UpdateShadow();
			}
		}

		private void UpdateShadow ()
		{
			var layer = ShadowView.Layer;

			layer.ShadowColor = PlatformElement.GetShadowColor(Element).ToCGColor();
			layer.ShadowOffset = PlatformElement.GetShadowOffset(Element).ToSizeF();
			layer.ShadowRadius = (nfloat)PlatformElement.GetShadowRadius(Element);
			layer.ShadowOpacity = (float)PlatformElement.GetShadowOpacity(Element);
		}
	}
}
