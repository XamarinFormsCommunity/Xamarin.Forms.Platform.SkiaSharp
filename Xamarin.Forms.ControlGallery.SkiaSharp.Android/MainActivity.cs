using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.SkiaSharp.Android;

namespace Xamarin.Forms.ControlGallery.SkiaSharp.Droid
{
	[Activity(Label = "Control Gallery", Icon = "@drawable/icon",
		 MainLauncher = true, HardwareAccelerated = true,
		 ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : SKApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Platform.SkiaSharp.Forms.Init();
            LoadApplication(new App());
        }
    }
}