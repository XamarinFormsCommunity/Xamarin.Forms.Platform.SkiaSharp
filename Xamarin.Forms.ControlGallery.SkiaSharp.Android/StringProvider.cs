using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.SkiaSharp.Android;

[assembly: Dependency(typeof(StringProvider))]
namespace Xamarin.Forms.ControlGallery.SkiaSharp.Android
{
	public class StringProvider : Controls.IStringProvider
	{
		public string CoreGalleryTitle
		{
			get
			{
				return "SkiaSharp.Android CoreGallery";
			}
		}
	}
}