
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.SkiaSharp.iOS;

[assembly: Dependency(typeof(StringProvider))]
namespace Xamarin.Forms.ControlGallery.SkiaSharp.iOS
{
	public class StringProvider : Controls.IStringProvider
	{
		public string CoreGalleryTitle
		{
			get
			{
				return "SkiaSharp.iOS CoreGallery";
			}
		}
	}
}