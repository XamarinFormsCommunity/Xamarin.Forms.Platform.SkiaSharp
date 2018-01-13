using SkiaSharp;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp.Renderers
{
    public class ImageRenderer : ViewRenderer<Image, Controls.Image>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new Controls.Image());
                }

                UpdateSource();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
                UpdateSource();
        }

        private async void UpdateSource()
        {
            var source = Element.Source;

            IImageSourceHandler handler;

            ((IImageController)Element).SetIsLoading(true);

            if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
            {
                SKBitmap image;

                try
                {
                    image = await handler.LoadImageAsync(source);
                }
                catch (OperationCanceledException)
                {
                    image = null;
                    Log.Warning("Image loading", "Image load cancelled");
                }
                catch (Exception ex)
                {
                    image = null;
                    Log.Warning("Image loading", $"Image load failed: {ex}");
                }

                Control.Source = image;
            }

            ((IImageController)Element).SetIsLoading(false);
        }
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken =
            default(CancellationToken), float scale = 1);
    }

    public sealed class StreamImageSourceHandler : IImageSourceHandler
    {
        public async Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken), float scale = 1)
        {
            SKBitmap image = null;

            var streamsource = imagesource as StreamImageSource;
            if (streamsource?.Stream == null) return null;
            using (
                var streamImage = await ((IStreamImageSource)streamsource)
                .GetStreamAsync(cancelationToken).ConfigureAwait(false))
            {
                if (streamImage != null)
                    image = SKBitmap.Decode(streamImage);
            }

            return image;
        }
    }

    public sealed class UriImageSourceHandler : IImageSourceHandler
    {
        public async Task<SKBitmap> LoadImageAsync(
            ImageSource imagesource,
            CancellationToken cancelationToken = default(CancellationToken),
            float scale = 1)
        {
            SKBitmap image = null;

            var imageLoader = imagesource as UriImageSource;

            if (imageLoader?.Uri == null)
                return null;

            using (Stream streamImage = await imageLoader.GetStreamAsync(cancelationToken))
            {
                if (streamImage == null || !streamImage.CanRead)
                {
                    return null;
                }

                image = SKBitmap.Decode(streamImage);
            }

            return image;
        }
    }
}
