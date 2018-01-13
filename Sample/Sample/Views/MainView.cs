using System;
using Xamarin.Forms;

namespace Sample.Views
{
    public class MainView : ContentPage
    {
        public MainView()
        {
            var panel = new StackLayout();

            var label = new Label
            {
                Text = "First Sample!",
                TextColor = Color.Red,
                Margin = new Thickness(12)
            };

            panel.Children.Add(label);

            var boxView = new BoxView
            {
                Color = Color.RoyalBlue,
                HeightRequest = 250,
                WidthRequest = 250,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(12, 24)
            };

            panel.Children.Add(boxView);

            var image = new Image
            {
                Source = new UriImageSource
                {
                    Uri = new Uri("http://xamarin.com/content/images/pages/branding/assets/xamagon.png"),
                    CachingEnabled = false
                },               
                Aspect = Aspect.AspectFit,
                HeightRequest = 250,
                WidthRequest = 250,
                Margin = new Thickness(12, 24)
            }; 

            panel.Children.Add(image);

            Content = panel;
        }
    }
}