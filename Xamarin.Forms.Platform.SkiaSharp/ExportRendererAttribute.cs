using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.SkiaSharp;
using Xamarin.Forms.Platform.SkiaSharp.Renderers;

[assembly: ExportRenderer(typeof(BoxView), typeof(BoxViewRenderer))]
[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]
[assembly: ExportRenderer(typeof(Page), typeof(PageRenderer))]

namespace Xamarin.Forms.Platform.SkiaSharp
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportRendererAttribute : HandlerAttribute
    {
        public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
        {
        }
    }
}