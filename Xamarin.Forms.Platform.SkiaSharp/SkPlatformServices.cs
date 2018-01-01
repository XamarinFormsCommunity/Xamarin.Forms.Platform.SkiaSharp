using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class SKPlatformServices : IPlatformServices
    {
        private static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider();

        public bool IsInvokeRequired => throw new NotImplementedException();

        public string RuntimePlatform => Device.Skia;

        public void BeginInvokeOnMainThread(Action action)
        {
            action.Invoke();
        }

        public Ticker CreateTicker()
        {
            throw new NotImplementedException();
        }

        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public string GetMD5Hash(string input)
        {
            var bytes = Checksum.ComputeHash(Encoding.UTF8.GetBytes(input));
            var ret = new char[32];
            for (var i = 0; i < 16; i++)
            {
                ret[i * 2] = (char)Hex(bytes[i] >> 4);
                ret[i * 2 + 1] = (char)Hex(bytes[i] & 0xf);
            }
            return new string(ret);
        }

        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            switch (size)
            {
                default:
                case NamedSize.Default:
                    return 16;
                case NamedSize.Micro:
                    return 9;
                case NamedSize.Small:
                    return 12;
                case NamedSize.Medium:
                    return 22;
                case NamedSize.Large:
                    return 32;
            }
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            throw new NotImplementedException();
        }

        public void OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void QuitApplication()
        {
            throw new NotImplementedException();
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            throw new NotImplementedException();
        }

        private static int Hex(int v)
        {
            if (v < 10)
                return '0' + v;
            return 'a' + v - 10;
        }
    }
}
