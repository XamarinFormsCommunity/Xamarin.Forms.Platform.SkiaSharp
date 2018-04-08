namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
	public class MasterDetailPageRenderer : VisualElementRenderer<MasterDetailPage, Native.MasterDetailPage>
    {
		public MasterDetailPageRenderer() => SetNativeControl(new Native.MasterDetailPage());
	}
}