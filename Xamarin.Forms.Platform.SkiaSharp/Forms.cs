using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public static class Forms
    {
        const string LogFormat = "[{0}] {1}";

        public static bool IsInitialized { get; private set; }

        public static void Init(IEnumerable<Assembly> rendererAssemblies = null)
        {
            if (IsInitialized)
                return;

            Log.Listeners.Add(new DelegateLogListener((c, m) => Debug.WriteLine(LogFormat, c, m)));

            Registrar.ExtraAssemblies = rendererAssemblies?.ToArray();

            IsInitialized = true;

            Device.PlatformServices = new SKPlatformServices();
            Device.Info = new SKDeviceInfo();

            ExpressionSearch.Default = new SKExpressionSearch();

            Registrar.RegisterAll(new[]
            {
                typeof(ExportRendererAttribute)
            });
        }
    }
}