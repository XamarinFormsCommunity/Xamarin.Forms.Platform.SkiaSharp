using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.ComponentModel;
using Views = Android.Views;

namespace Xamarin.Forms.Platform.SkiaSharp.Android
{
    public class SKApplicationActivity : Activity
    {
        Application _application;
        Platform _canvas;
        LinearLayout _layout;

        protected void LoadApplication(Application application)
        {
            _application = application ?? throw new ArgumentNullException("application");

            application.PropertyChanged += AppOnPropertyChanged;

            SetMainPage();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			var decorView = Window.DecorView;

			// Hide the status bar.
			var uiOptions = Views.SystemUiFlags.LayoutFullscreen | Views.SystemUiFlags.LayoutStable;
			decorView.SystemUiVisibility = (Views.StatusBarVisibility)uiOptions;

			Window.SetStatusBarColor(global::Android.Graphics.Color.Transparent);
			
			// Remember that you should never show the action bar if the
			// status bar is hidden.
			ActionBar.Hide();

			_layout = new LinearLayout(BaseContext);
            SetContentView(_layout);
        }

        void AppOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "MainPage")
                InternalSetPage(_application.MainPage);
        }

        void SetMainPage()
        {
            InternalSetPage(_application.MainPage);
        }

        void InternalSetPage(Page page)
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