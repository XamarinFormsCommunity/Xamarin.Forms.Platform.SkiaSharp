using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.SkiaSharp.Android
{
    public class SKApplicationActivity : Activity
    {
        private Application _application;
        private Platform _canvas;
        private LinearLayout _layout;

        protected void LoadApplication(Application application)
        {
            _application = application ?? throw new ArgumentNullException("application");

            application.PropertyChanged += AppOnPropertyChanged;

            SetMainPage();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _layout = new LinearLayout(BaseContext);
            SetContentView(_layout);
        }

        private void AppOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "MainPage")
                InternalSetPage(_application.MainPage);
        }

        private void SetMainPage()
        {
            InternalSetPage(_application.MainPage);
        }

        private void InternalSetPage(Page page)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Call Forms.Init (Activity, Bundle) before this");

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
            var renderer = new SKViewRenderer(view, this);
            _layout.AddView(renderer);
        }
    }
}