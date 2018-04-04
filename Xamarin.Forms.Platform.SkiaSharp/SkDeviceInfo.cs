using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class SKDeviceInfo : DeviceInfo
    {
        public override Size PixelScreenSize
        {
            get
            {
                return new Size();
            }
        }

        public override Size ScaledScreenSize
        {
            get
            {
                return new Size();
            }
        }

        public override double ScalingFactor
        {
            get
            {
                return 1.0d;
            }
        }
    }
}