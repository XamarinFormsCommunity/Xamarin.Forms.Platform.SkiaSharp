using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp.Droid
{
    internal class ResourcesProvider : ISystemResourcesProvider
    {
        public IResourceDictionary GetSystemResources()
        {
            return new ResourceDictionary();
        }
    }
}