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
                TextColor = Color.Red
            };

            panel.Children.Add(label);

            Content = panel;
        }
    }
}