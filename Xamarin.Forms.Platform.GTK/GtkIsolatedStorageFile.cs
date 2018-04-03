﻿using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.GTK
{
    public class GtkIsolatedStorageFile : IIsolatedStorageFile
    {
        public Task CreateDirectoryAsync(string path)
        {
            var storage = CreateStorageFileInstance();

            storage.CreateDirectory(path);
            return Task.FromResult(true);
        }

        public Task<bool> GetDirectoryExistsAsync(string path)
        {
            var storage = CreateStorageFileInstance();

            return Task.FromResult(storage.DirectoryExists(path));
        }

        public Task<bool> GetFileExistsAsync(string path)
        {
            var storage = CreateStorageFileInstance();

            return Task.FromResult(storage.FileExists(path));
        }

        public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
        {
            var storage = CreateStorageFileInstance();

            return Task.FromResult(storage.GetLastWriteTime(path));
        }

        public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
        {
            var storage = CreateStorageFileInstance();

            Stream stream = storage.OpenFile(path, mode, access);
            return Task.FromResult(stream);
        }

        public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
        {
            var storage = CreateStorageFileInstance();

            Stream stream = storage.OpenFile(path, mode, access, share);
            return Task.FromResult(stream);
        }

        private IsolatedStorageFile CreateStorageFileInstance()
        {
            return IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly,
                    null, null);
        }
    }
}
