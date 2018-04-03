﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Internals;
using WBinding = Windows.UI.Xaml.Data.Binding;
using WBindingExpression = Windows.UI.Xaml.Data.BindingExpression;

namespace Xamarin.Forms.Platform.UWP
{
	internal static class FrameworkElementExtensions
	{
		static readonly Lazy<ConcurrentDictionary<Type, DependencyProperty>> ForegroundProperties =
			new Lazy<ConcurrentDictionary<Type, DependencyProperty>>(() => new ConcurrentDictionary<Type, DependencyProperty>());

		public static Brush GetForeground(this FrameworkElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			return (Brush)element.GetValue(GetForegroundProperty(element));
		}

		public static WBinding GetForegroundBinding(this FrameworkElement element)
		{
			WBindingExpression expr = element.GetBindingExpression(GetForegroundProperty(element));
			if (expr == null)
				return null;

			return expr.ParentBinding;
		}

		public static object GetForegroundCache(this FrameworkElement element)
		{
			WBinding binding = GetForegroundBinding(element);
			if (binding != null)
				return binding;

			return GetForeground(element);
		}

		public static void RestoreForegroundCache(this FrameworkElement element, object cache)
		{
			var binding = cache as WBinding;
			if (binding != null)
				SetForeground(element, binding);
			else
				SetForeground(element, (Brush)cache);
		}

		public static void SetForeground(this FrameworkElement element, Brush foregroundBrush)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			element.SetValue(GetForegroundProperty(element), foregroundBrush);
		}

		public static void SetForeground(this FrameworkElement element, WBinding binding)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			element.SetBinding(GetForegroundProperty(element), binding);
		}

		internal static IEnumerable<T> GetDescendantsByName<T>(this DependencyObject parent, string elementName) where T : DependencyObject
		{
			int myChildrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < myChildrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
				if (controlName == elementName && child is T)
					yield return child as T;
				else
				{
					foreach (var subChild in child.GetDescendantsByName<T>(elementName))
						yield return subChild;
				}
			}
		}

		internal static T GetFirstDescendant<T>(this DependencyObject element) where T : FrameworkElement
		{
			int count = VisualTreeHelper.GetChildrenCount(element);
			for (var i = 0; i < count; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, i);

				T target = child as T ?? GetFirstDescendant<T>(child);
				if (target != null)
					return target;
			}

			return null;
		}

		static DependencyProperty GetForegroundProperty(FrameworkElement element)
		{
			if (element is Control)
				return Control.ForegroundProperty;
			if (element is TextBlock)
				return TextBlock.ForegroundProperty;

			Type type = element.GetType();

			DependencyProperty foregroundProperty;
			if (!ForegroundProperties.Value.TryGetValue(type, out foregroundProperty))
			{
				FieldInfo field = ReflectionExtensions.GetFields(type).FirstOrDefault(f => f.Name == "ForegroundProperty");
				if (field == null)
					throw new ArgumentException("type is not a Foregroundable type");

				var property = (DependencyProperty)field.GetValue(null);
				ForegroundProperties.Value.TryAdd(type, property);

				return property;
			}

			return foregroundProperty;
		}

		internal static IEnumerable<T> GetChildren<T>(this DependencyObject parent) where T : DependencyObject
		{
			int myChildrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < myChildrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is T)
					yield return child as T;
				else
				{
					foreach (var subChild in child.GetChildren<T>())
						yield return subChild;
				}
			}
		}
	}
}