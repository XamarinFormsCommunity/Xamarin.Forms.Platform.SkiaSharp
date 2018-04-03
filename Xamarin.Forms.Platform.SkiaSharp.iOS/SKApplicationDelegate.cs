using Foundation;
using System;
using System.ComponentModel;
using UIKit;

namespace Xamarin.Forms.Platform.SkiaSharp.iOS
{
    public class SKApplicationDelegate : UIApplicationDelegate
    {
        private Application _application;
        private UIWindow _window;
        private Platform _canvas;

        public override UIWindow Window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            if (Window == null)
                Window = new UIWindow(UIScreen.MainScreen.Bounds);

            if (_application == null)
                throw new InvalidOperationException("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");

            SetMainPage();
            _application.SendStart();
            return true;
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            if (_application != null)
            {
                _application.SendResume();
            }
        }

        public override async void OnResignActivation(UIApplication uiApplication)
        {
            if (_application != null)
            {
                await _application.SendSleepAsync();
            }
        }

        protected void LoadApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            Application.SetCurrentApplication(application);
            _application = application;

            application.PropertyChanged += ApplicationOnPropertyChanged;
        }

        private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "MainPage")
                UpdateMainPage();
        }

        private void SetMainPage()
        {
            UpdateMainPage();
            Window.MakeKeyAndVisible();
        }

        private void UpdateMainPage()
        {
            if (_application.MainPage == null)
                return;

            var page = _application.MainPage;

            if (_canvas != null)
            {
                _canvas.SetPage(page);
                return;
            }

            _canvas = new Platform();
            var platformRenderer = new PlatformRenderer(_canvas);

            if (_application != null)
                _application.Platform = _canvas;

            _canvas.SetPage(page);

            var view = _canvas.PlatformRenderer;
            var renderer = new SKViewRenderer(view);

            Window.RootViewController = new UIViewController
            {
                View = renderer
            };
        }
    }
}