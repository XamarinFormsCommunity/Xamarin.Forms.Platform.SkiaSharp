using SkiaSharp;
using System;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class ControlManager 
    {
        public event EventHandler OnInvalidate;

        public ControlManager()
        {
            Controls = new ControlCollection(this);
        }

        public ControlCollection Controls { get; }

        public void Invalidate()
        {
            OnInvalidate?.Invoke(this, EventArgs.Empty);         
        }

        public void Clear(SKCanvas canvas)
        {
            canvas.Clear();
        }

        public void Draw(SKCanvas canvas)
        {
            foreach (var control in Controls)
            {
                control.Draw(canvas);
            }
        }
    }
}
