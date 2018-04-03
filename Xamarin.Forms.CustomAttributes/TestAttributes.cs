using System;
using System.Diagnostics;

namespace Xamarin.Forms.CustomAttributes
{
	[Conditional ("DEBUG")]
	[AttributeUsage (
		AttributeTargets.Class |
		AttributeTargets.Event | 
		AttributeTargets.Property |
		AttributeTargets.Method |
		AttributeTargets.Delegate, 
		AllowMultiple = true
		)]
	public class PlatformAttribute : Attribute
	{
		readonly string _platform;
		public PlatformAttribute (object platform)
		{
			_platform = platform.ToString ();
		}

		public string Platform => "Issue: " + _platform;
	}

	public enum IssueTracker
	{
		Github, 
		Bugzilla,
		None
	}

	public enum NavigationBehavior
	{
		PushAsync,
		PushModalAsync,
		SetApplicationRoot,
		Default
	}

	[Conditional ("DEBUG")]
	[AttributeUsage (
		AttributeTargets.Class |
		AttributeTargets.Method,
		AllowMultiple = true
		)]
	public class IssueAttribute : Attribute
	{
		bool _modal;

		public IssueAttribute (IssueTracker issueTracker, int issueNumber, string description, 
			NavigationBehavior navigationBehavior = NavigationBehavior.Default, int issueTestNumber = 0)
		{
			IssueTracker = issueTracker;
			IssueNumber = issueNumber;
			Description = description;
			PlatformsAffected = PlatformAffected.Default;
			NavigationBehavior = navigationBehavior;
			IssueTestNumber = issueTestNumber;
		}

		public IssueAttribute (IssueTracker issueTracker, int issueNumber, string description, 
			PlatformAffected platformsAffected, NavigationBehavior navigationBehavior = NavigationBehavior.Default, 
			int issueTestNumber = 0)
		{
			IssueTracker = issueTracker;
			IssueNumber = issueNumber;
			Description = description;
			PlatformsAffected = platformsAffected;
			NavigationBehavior = navigationBehavior;
			IssueTestNumber = issueTestNumber;
		}

		public IssueTracker IssueTracker { get; }

		public int IssueNumber { get; }

		public int IssueTestNumber { get; }

		public string Description { get; }

		public PlatformAffected PlatformsAffected { get; }

		public NavigationBehavior NavigationBehavior { get; }

		public string DisplayName => IssueTestNumber == 0
			? $"{IssueTracker.ToString().Substring(0, 1)}{IssueNumber}"
			: $"{IssueTracker.ToString().Substring(0, 1)}{IssueNumber} ({IssueTestNumber})";
	}

	[Conditional ("DEBUG")]
	public class UiTestExemptAttribute : Attribute
	{
		// optional string reason
		readonly string _exemptReason;
		readonly string _description;

		public UiTestExemptAttribute (ExemptReason exemptReason, string description = null)
		{
			_exemptReason = Enum.GetName (typeof(ExemptReason), exemptReason);
			_description = description;
		}

		public string ExemptReason => "Exempt: " + _exemptReason;

		public string Description => "Description: " + _description;
	}

	[Conditional ("DEBUG")]
	public class UiTestFragileAttribute : Attribute
	{
		// optional string reason
		readonly string _description;

		public UiTestFragileAttribute (string description = null)
		{
			_description = description;
		}

		public string Description => "Description: " + _description;
	}

	[Conditional ("DEBUG")]
	[AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
	public class UiTestBrokenAttribute : Attribute
	{
		// optional string reason
		readonly string _exemptReason;
		readonly string _description;

		public UiTestBrokenAttribute (BrokenReason exemptReason, string description = null)
		{
			_exemptReason = Enum.GetName (typeof(ExemptReason), exemptReason);
			_description = description;
		}

		public string ExemptReason => "Exempt: " + _exemptReason;

		public string Description => "Description: " + _description;
	}

	[Flags]
	public enum PlatformAffected
	{
		iOS = 1 << 0,
		Android = 1 << 1,
		WinPhone = 1 << 2,
		WinRT = 1 << 3,
		UWP = 1 << 4,
		WPF = 1 << 5,
		All = ~0,
		Default = 0
	}

	public enum ExemptReason
	{
		None,
		AbstractClass,
		IsUnitTested,
		NeedsUnitTest,
		BaseClass,
		TimeConsuming,
		CannotTest
	}

	public enum BrokenReason 
	{
		UITestBug,
		CalabashBug,
		CalabashUnsupported,
		CalabashiOSUnsupported,
		CalabashAndroidUnsupported,
	}

	public static class Test
	{
		public enum Features
		{
			Binding,
			XAML,
			Maps
		}

		public enum Views
		{
			Label,
			TableView,
			SwitchCell,
			ViewCell,
			Image,
			ListView,
			ScrollView,
			Switch,
			Button,
			TextCell,
			Entry,
			SearchBar,
			ImageCell,
			EntryCell,
			Editor,
			DatePicker
		}

		public enum Layouts
		{
			StackLayout,
			Grid
		}

		public enum Pages
		{
			NavigationPage,
			MasterDetailPage,
			TabbedPage,
			ContentPage,
			CarouselPage
		}

		public enum Button
		{
			Clicked,
			Command,
			Text,
			TextColor,
			Font,
			BorderWidth,
			BorderColor,
			BorderRadius,
			Image,
		}

		public enum VisualElement
		{
			IsEnabled,
			Navigation,
			InputTransparent,
			Layout,
			X,
			Y,
			AnchorX,
			AnchorY,
			TranslationX,
			TranslationY,
			Width,
			Height,
			Bounds,
			Rotation,
			RotationX,
			RotationY,
			Scale,
			IsVisible,
			Opacity,
			BackgroundColor,
			IsFocused,
			Focus,
			Unfocus,
			Focused,
			Unfocused,
			Default
		}

		public enum Cell
		{
			Tapped,
			Appearing,
			Disappearing,
			IsEnabled,
			RenderHeight,
			Height,
			ContextActions,
		}

		public enum EntryCell
		{
			Completed,
			Text,
			Label,
			Placeholder,
			LabelColor,
			Keyboard,
			XAlign,
		}

		public enum TextCell
		{
			Command,
			Text,
			Detail,
			TextColor,
			DetailColor,
		}

		public enum ImageCell
		{
			ImageSource,
		}

		public enum GestureRecognizer
		{
			Parent,
		}

		public enum Binding
		{
			Path,
			Converter,
			ConverterParameter,
			Create,
		}

		public enum TapGestureRecognizer
		{
			Tapped,
			Command,
			CommandParameter,
			NumberOfTapsRequired,
			TappedCallBack,
			TappedCallBackParameter
		}

		public enum Device
		{
			OS,
			Idiom,
			OnPlatform,
			OnPlatformGeneric,
			OpenUri,
			BeginInvokeOnMainThread,
			StartTimer,
			Styles
		}

		public enum ValueChangedEventArgs
		{
			OldValue,
			NewValue,
		}

		public enum View
		{
			GestureRecognizers,
		}

		public enum Page
		{
			BackgroundImage,
			ToolbarItems,
			IsBusy,
			DisplayAlert,
			DisplayAlertAccept,
			DisplayActionSheet,
			Title,
			Icon,
			Appearing,
			Disappearing,
		}

		public enum NavigationPage
		{
			GetBackButtonTitle,
			SetBackButtonTitle,
			GetHasNavigationBar,
			SetHasNavigationBar,
			Tint,
			BarBackgroundColor,
			BarTextColor,
			BarGetTitleIcon,
			BarSetTitleIcon,
			Popped,
			PushAsync,
			PopAsync,
			PopToRootAsync,
		}

		public enum MultiPage
		{
			CurrentPageChanged,
			CurrentPagesChanged,
			ItemsSource,
			ItemTemplate,
			SelectedItem,
			CurrentPage,
			Children,
		}

		public enum MenuItem
		{
			Text,
			Command,
			IsDestructive,
			Icon,
			Clicked,
			IsEnabled
		}

		public enum ToolbarItem
		{
			Activated,
			Name,
			Order,
			Priority
		}

		public enum SwitchCell
		{
			OnChanged,
			On,
			Text,
		}

		public enum ViewCell
		{
			View,
		}

		public enum ListView
		{
			ItemAppearing,
			ItemDisappearing,
			ItemSelected,
			ItemTapped,
			SelectedItem,
			HasUnevenRows,
			RowHeight,
			GroupHeaderTemplate,
			IsGroupingEnabled,
			GroupDisplayBinding,
			GroupShortNameBinding,
			ScrollTo,
			FastScroll
		}

		public enum TableView
		{
			Root,
			Intent,
			RowHeight,
			HasUnevenRows,
		}

		public enum TableSectionBase
		{
			Title,
		}

		public enum Layout
		{
			IsClippedToBounds,
			Padding,
			RaiseChild,
			LowerChild,
			GenericChildren,
		}

		public enum AbsoluteLayout
		{
			Children,
			SetLayoutFlags,
			SetLayoutBounds,
		}

		public enum ActivityIndicator
		{
			IsRunning,
			Color,
		}

		public enum ContentView
		{
			View,
		}

		public enum DatePicker
		{
			DateSelected,
			Format,
			Date,
			MinimumDate,
			MaximumDate,
			Focus,
			IsVisible,
			TextColor,
			FontAttributes,
			FontFamily,
			FontSize
		}

		public enum InputView
		{
			Keyboard,
		}

		public enum Editor
		{
			Completed,
			TextChanged,
			Text,
			TextColor,
			FontAttributes,
			FontFamily,
			FontSize,
			MaxLength
		}

		public enum Entry
		{
			Completed,
			TextChanged,
			Placeholder,
			IsPassword,
			Text,
			TextColor,
			Keyboard,
			HorizontalTextAlignmentStart,
			HorizontalTextAlignmentCenter,
			HorizontalTextAlignmentEnd,
			HorizontalTextAlignmentPlaceholderStart,
			HorizontalTextAlignmentPlaceholderCenter,
			HorizontalTextAlignmentPlaceholderEnd,
			FontAttributes,
			FontFamily,
			FontSize,
			PlaceholderColor,
			TextDisabledColor,
			PlaceholderDisabledColor,
			PasswordColor,
			MaxLength
		}

		public enum Frame
		{
			OutlineColor,
			HasShadow
		}

		public enum Image
		{
			Source,
			Aspect,
			IsOpaque,
			IsLoading,
			AspectFill,
			AspectFit,
			Fill
		}

		public enum ImageSource
		{
			FromFile,
			FromStream,
			FromResource,
			FromUri,
			Cancel,
		}

		public enum UriImageSource
		{
			Uri,
			CachingEnabled,
			CacheValidity,
		}

		public enum Keyboard
		{
			Create,
			Default,
			Email,
			Text,
			Url,
			Numeric,
			Telephone,
			Chat,
		}

		public enum Label
		{
			TextColor,
			Text,
			FormattedText,
			FontAttibutesBold,
			FontAttributesItalic,
			FontNamedSizeMicro,
			FontNamedSizeSmall,
			FontNamedSizeMedium,
			FontNamedSizeLarge,
			LineBreakModeNoWrap,
			LineBreakModeWordWrap,
			LineBreakModeCharacterWrap,
			LineBreakModeHeadTruncation,
			LineBreakModeTailTruncation,
			LineBreakModeMiddleTruncation,
			HorizontalTextAlignmentStart,
			HorizontalTextAlignmentCenter,
			HorizontalTextAlignmentEnd,
			VerticalTextAlignmentStart,
			VerticalTextAlignmentCenter,
			VerticalTextAlignmentEnd,
		}

		public enum MasterDetailPage {
			Master,
			Detail,
			IsGestureEnabled,
			IsPresented,
			MasterBehavior
		}

		public enum OpenGlView {
			OnDisplay,
			HasRenderLoop,
			Display
		}

		public enum ProgressBar {
			Progress,
			ProgressColor
		}

		public enum RelativeLayout {
			Children,
			SetBoundsConstraint
		}

		public enum ScrollView {
			ContentSize,
			Orientation,
			Content
		}

		public enum SearchBar
		{
			SearchButtonPressed,
			TextChanged,
			SearchCommand,
			Text,
			PlaceHolder,
			CancelButtonColor,
			FontAttributes,
			FontFamily,
			FontSize,
			TextAlignmentStart,
			TextAlignmentCenter,
			TextAlignmentEnd,
			PlaceholderAlignmentStart,
			PlaceholderAlignmentCenter,
			PlaceholderAlignmentEnd,
			TextColor,
			PlaceholderColor
		}

		public enum Slider {
			Minimum,
			Maximum,
			Value,
			MinimumTrackColor,
			MaximumTrackColor,
			ThumbColor,
			ThumbImage
		}

		public enum StackLayout {
			Orientation,
			Spacing
		}

		public enum Stepper {
			Maximum,
			Minimum,
			Value,
			Increment
		}

		public enum Switch {
			IsToggled
		}

		public enum TimePicker {
			Format,
			Time,
			Focus,
			TextColor,
			FontAttributes,
			FontFamily,
			FontSize
		}

		public enum WebView {
			UrlWebViewSource,
			HtmlWebViewSource,
			LoadHtml,
			MixedContentDisallowed,
			MixedContentAllowed,
			JavaScriptAlert,
			EvaluateJavaScript
		}

		public enum UrlWebViewSource {
			Url
		}

		public enum HtmlWebViewSource {
			BaseUrl,
			Html
		}

		public enum Grid {
			Children,
			SetRow,
			SetRowSpan,
			SetColumn,
			SetColumnSpan,
			RowSpacing,
			ColumnSpacing,
			ColumnDefinitions,
			RowDefinitions
		}

		public enum ContentPage {
			Content
		}

		public enum Picker {
			Title,
			Items,
			SelectedIndex,
			Focus,
			TextColor,
			FontAttributes,
			FontFamily,
			FontSize
		}

		public enum FileImageSource {
			File,
			Cancel
		}

		public enum StreamImageSource {
			Stream
		}

		public enum OnPlatform {
			WinPhone,
			Android,
			iOS
		}

		public enum OnIdiom {
			Phone,
			Tablet
		}

		public enum Span {
			Text,
			ForeGroundColor,
			BackgroundColor,
			Font,
			PropertyChanged
		}

		public enum FormattedString {
			ToStringOverride,
			Spans,
			PropertyChanged
		}

		public enum BoxView
		{
			Color
		}

	}
}

