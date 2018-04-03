using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class ButtonCoreGalleryPage : CoreGalleryPage<Button>
	{
		protected override bool SupportsTapGestureRecognizer {
			get { return false; }
		}

		protected override bool SupportsFocus {
			get { return false; }
		}

		protected override void InitializeElement (Button element)
		{
			element.Text = "Button";
		}

		protected override void Build (StackLayout stackLayout)
		{
			base.Build (stackLayout);

			IsEnabledStateViewContainer.View.Clicked += (sender, args) => IsEnabledStateViewContainer.TitleLabel.Text += " (Tapped)";

			var borderButtonContainer = new ViewContainer<Button> (Test.Button.BorderColor, 
				new Button {
					Text = "BorderColor",
					BackgroundColor = Color.Transparent, 
					BorderColor = Color.Red, 
					BorderWidth = 1,
				}
			);

			var borderRadiusContainer = new ViewContainer<Button> (Test.Button.BorderRadius, 
				new Button {
					Text = "BorderRadius",
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Red,
#pragma warning disable 0618
					BorderRadius = 20,
#pragma warning restore
					BorderWidth = 1,
				}
			);
		
			var borderWidthContainer = new ViewContainer<Button> (Test.Button.BorderWidth,
				new Button {
					Text = "BorderWidth",
					BackgroundColor = Color.Transparent,
					BorderColor = Color.Red,
					BorderWidth = 15,
				}
			);

			var clickedContainer = new EventViewContainer<Button> (Test.Button.Clicked, 
				new Button {
					Text = "Clicked"
				}
			);
			clickedContainer.View.Clicked += (sender, args) => clickedContainer.EventFired ();

			var commandContainer = new ViewContainer<Button> (Test.Button.Command, 
				new Button {
					Text = "Command", 
					Command = new Command (() => DisplayActionSheet ("Hello Command", "Cancel", "Destroy"))
				}
			);

			var fontContainer = new ViewContainer<Button> (Test.Button.Font,
				new Button {
					Text = "Font", 
					Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold) 
				}
			);

			var imageContainer = new ViewContainer<Button> (Test.Button.Image, 
				new Button {
					Text = "Image", 
					Image = new FileImageSource { File = "bank.png" }
				}
			)
			;
			var textContainer = new ViewContainer<Button> (Test.Button.Text, 
				new Button {
					Text = "Text"
				}
			);

			var textColorContainer = new ViewContainer<Button> (Test.Button.TextColor, 
				new Button {
					Text = "TextColor", TextColor = Color.Pink
				}
			);

			Add (borderButtonContainer);
			Add (borderRadiusContainer);
			Add (borderWidthContainer);
			Add (clickedContainer);
			Add (commandContainer);
			Add (fontContainer);
			Add (imageContainer);
			Add (textContainer);
			Add (textColorContainer);
			//stackLayout.Children.Add (textColorContainer);
		}
	}
}