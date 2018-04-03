using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
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
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.BeginGetResponse(ar =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.SetCanceled();
                        return;
                    }

                    try
                    {
                        Stream stream = request.EndGetResponse(ar).GetResponseStream();
                        tcs.TrySetResult(stream);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            return new SKIsolatedStorageFile(IsolatedStorageFile.GetUserStoreForApplication());
        }

        public void OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void QuitApplication()
        {
            throw new NotImplementedException();
        }

		void TimerCallback(object state)
		{
			var callback = state as Func<bool>;
			callback?.Invoke();
		}

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
			new Timer(TimerCallback, callback, Convert.ToInt32(interval.TotalMilliseconds), Timeout.Infinite);
		}

        private static int Hex(int v)
        {
            if (v < 10)
                return '0' + v;
            return 'a' + v - 10;
        }
    }

    public class SKIsolatedStorageFile : IIsolatedStorageFile
    {
        readonly IsolatedStorageFile _isolatedStorageFile;

        public SKIsolatedStorageFile(IsolatedStorageFile isolatedStorageFile)
        {
            _isolatedStorageFile = isolatedStorageFile;
        }

        public Task CreateDirectoryAsync(string path)
        {
            _isolatedStorageFile.CreateDirectory(path);
            return Task.FromResult(true);
        }

        public Task<bool> GetDirectoryExistsAsync(string path)
        {
            return Task.FromResult(_isolatedStorageFile.DirectoryExists(path));
        }

        public Task<bool> GetFileExistsAsync(string path)
        {
            return Task.FromResult(_isolatedStorageFile.FileExists(path));
        }

        public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
        {
            return Task.FromResult(_isolatedStorageFile.GetLastWriteTime(path));
        }

        public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
        {
            Stream stream = _isolatedStorageFile.OpenFile(path, mode, access);
            return Task.FromResult(stream);
        }

        public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
        {
            Stream stream = _isolatedStorageFile.OpenFile(path, mode, access, share);
            return Task.FromResult(stream);
        }
    }
}
