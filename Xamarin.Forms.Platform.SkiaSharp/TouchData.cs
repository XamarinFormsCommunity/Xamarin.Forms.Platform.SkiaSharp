namespace Xamarin.Forms.Platform.SkiaSharp
{
	public class TouchData
    {
		public Point Point { get; set; }
		public TouchAction Action { get; set; }
		public bool Handled { get; set; }
	}

	public enum TouchAction
	{
		None,
		Tap
	}

}
