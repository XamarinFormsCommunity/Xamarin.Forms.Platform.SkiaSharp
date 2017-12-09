using SkiaSharp.Views.Forms;
using System;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class View : SKCanvasView
    {
        private ControlManager _manager;

        public View()
        {
            _manager = new ControlManager();

            _manager.OnInvalidate += delegate (object sender, EventArgs e)
            {
                InvalidateSurface();
            };
        }

        public ControlManager Manager { get => _manager; }

        public ControlCollection Controls => _manager.Controls;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            _manager.Clear(e.Surface.Canvas);

            base.OnPaintSurface(e);

            _manager.Draw(e.Surface.Canvas);
        }
    }
}