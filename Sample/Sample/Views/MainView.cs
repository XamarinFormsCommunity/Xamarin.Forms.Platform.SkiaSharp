using Xamarin.Forms;

namespace Sample.Views
{
    public class MainView : ContentPage
    {
        public MainView()
        {
            var label = new Label
            {
                Text = "First Sample!",
                FontSize = 24
            };

            Content = label;
        }
    }
}