using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    // TODO: Move to every platform project 
    public static class SkiaApplication
    {
        private static Application _application;

        public static void LoadApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            Application.SetCurrentApplication(application);
            _application = application;

            application.PropertyChanged += ApplicationOnPropertyChanged;
            UpdateMainPage();
        }

        private static void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Application.MainPage))
            {
                UpdateMainPage();
            }
        }

        private static void UpdateMainPage()
        {
            if (_application.MainPage == null)
                return;

            var platform = new Platform();
            platform.SetPage(_application.MainPage);
        }
    }
}