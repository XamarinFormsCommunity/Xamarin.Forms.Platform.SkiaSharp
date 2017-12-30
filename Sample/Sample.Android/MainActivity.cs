using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.SkiaSharp.Droid;

namespace Sample.Droid
{
    [Activity(
        Label = "Sample", 
        MainLauncher = true)]
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