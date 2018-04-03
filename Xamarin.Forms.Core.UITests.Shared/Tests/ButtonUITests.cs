using NUnit.Framework;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Core.UITests
{
	[TestFixture]
	[Category(UITestCategories.Button)]
	internal class ButtonUITests : _ViewUITests
	{
		public ButtonUITests()
		{
			PlatformViewType = Views.Button;
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.ButtonGallery);
		}

		// View Tests
		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction")]
		public override void _Focus()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction")]
		public override void _GestureRecognizers()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction")]
		public override void _IsFocused()
		{
		}

		[UiTestExempt(ExemptReason.CannotTest, "Invalid interaction")]
		public override void _UnFocus()
		{
		}

		// Button Tests
		[Test]
		[UiTest(typeof(Button), "BorderColor")]
		[UiTestBroken(BrokenReason.CalabashAndroidUnsupported, "Figure out how to get Android Drawables")]
		[UiTestBroken(BrokenReason.CalabashiOSUnsupported, "iOS nil result")]
		public void BorderColor()
		{
			//TODO iOS
			var remote = new ViewContainerRemote(App, Test.Button.BorderColor, PlatformViewType);
			remote.GoTo();
		}

		[Test]
		[UiTest(typeof(Button), "BorderRadius")]
		[UiTestBroken(BrokenReason.CalabashAndroidUnsupported, "Figure out how to get Android Drawables")]
		public void BorderRadius()
		{
			var remote = new ViewContainerRemote(App, Test.Button.BorderRadius, PlatformViewType);
			remote.GoTo();

#if __IOS__
			var borderRadius = remote.GetProperty<float>(Button.CornerRadiusProperty);
			Assert.AreEqual(20.0f, borderRadius);
#endif
		}

		[Test]
		[UiTest(typeof(Button), "BorderWidth")]
		[UiTestBroken(BrokenReason.CalabashAndroidUnsupported, "Figure out how to get Android Drawables")]
		public void BorderWidth()
		{
			var remote = new ViewContainerRemote(App, Test.Button.BorderWidth, PlatformViewType);
			remote.GoTo();

#if __IOS__
			var borderWidth = remote.GetProperty<float>(Button.BorderWidthProperty);
			Assert.AreEqual(15.0f, borderWidth);
#endif
		}

		[Test]
		[UiTest(typeof(Button), "Clicked")]
		public void Clicked()
		{
			var remote = new EventViewContainerRemote(App, Test.Button.Clicked, PlatformViewType);
			remote.GoTo();

			var textBeforeClick = remote.GetEventLabel().Text;
			Assert.AreEqual("Event: Clicked (none)", textBeforeClick);

			// Click Button
			remote.TapView();

			var textAfterClick = remote.GetEventLabel().Text;
			Assert.AreEqual("Event: Clicked (fired 1)", textAfterClick);
		}

		[Test]
		[UiTest(typeof(Button), "Command")]
		public void Command()
		{
			var remote = new ViewContainerRemote(App, Test.Button.Command, PlatformViewType);
			remote.GoTo();

			remote.TapView();

			App.WaitForElement(q => q.Marked("Hello Command"));
			App.Tap(q => q.Marked("Destroy"));
		}

		[Test]
		[UiTest(typeof(Button), "Font")]
		public void Font()
		{
			//TODO iOS
			var remote = new ViewContainerRemote(App, Test.Button.Font, PlatformViewType);
			remote.GoTo();

#if __ANDROID__
			var isBold = remote.GetProperty<bool> (Button.FontProperty);
			Assert.True (isBold);
#elif __MACOS__
			Assert.Inconclusive("needs testing");
#else
			var font = remote.GetProperty<Font>(Button.FontProperty);
			Assert.True(font.FontAttributes.HasFlag(FontAttributes.Bold));
#endif
		}

		[Test]
		[UiTest(typeof(Button), "Image")]
		[UiTestExempt(ExemptReason.TimeConsuming, "Need way to check Android resources")]
		public void Image()
		{
			//TODO iOS
			var remote = new ViewContainerRemote(App, Test.Button.Image, PlatformViewType);
			remote.GoTo();
		}

		[Test]
		[UiTest(typeof(Button), "Text")]
		public void Text()
		{
			var remote = new ViewContainerRemote(App, Test.Button.Text, PlatformViewType);
			remote.GoTo();

			var buttonText = remote.GetProperty<string>(Button.TextProperty);
			Assert.AreEqual("Text", buttonText);
		}

		//TODO iOS

#if __ANDROID__
		[Test]
		[UiTest (typeof (Button), "TextColor")]
		public void TextColor ()
		{
			var remote = new ViewContainerRemote (App, Test.Button.TextColor, PlatformViewType);
			remote.GoTo ();

			var buttonTextColor = remote.GetProperty<Color> (Button.TextColorProperty);
			Assert.AreEqual (Color.Pink, buttonTextColor);
		}
#endif

		protected override void FixtureTeardown()
		{
			App.NavigateBack();
			base.FixtureTeardown();
		}
	}
}