using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp.iOS
{
    internal class ResourcesProvider : ISystemResourcesProvider
    {
        public IResourceDictionary GetSystemResources()
        {
            return new ResourceDictionary();
        }
    }
}