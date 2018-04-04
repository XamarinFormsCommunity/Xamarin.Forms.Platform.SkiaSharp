namespace Xamarin.Forms.Platform.SkiaSharp
{
	public struct TouchData
    {
		public Point Point;
		public TouchAction Action;

    }

	public enum TouchAction
	{
		None,
		Tap
	}

}
