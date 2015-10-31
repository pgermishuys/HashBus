﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HashBus.Projection
{
    public sealed class FileListRepository<TValue> : IRepository<string, IEnumerable<TValue>>, IDisposable
    {
        private readonly string folderName;
        private readonly Mutex @lock;

        public FileListRepository(string folderName)
        {
            this.folderName = folderName;
            this.@lock = new Mutex(false, folderName.Replace('\\', '/'));
        }

        public Task<IEnumerable<TValue>> GetAsync(string key)
        {
            var fileName = Path.Combine(this.folderName, $"{key}.json");
            if (!File.Exists(fileName))
            {
                return Task.FromResult(Enumerable.Empty<TValue>());
            }

            try
            {
                this.@lock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }

            try
            {
                using (var textReader = new StreamReader(fileName))
                {
                    return Task.FromResult(
                        new JsonSerializer().Deserialize<IEnumerable<TValue>>(new JsonTextReader(textReader)) ?? Enumerable.Empty<TValue>());
                }
            }
            finally
            {
                this.@lock.ReleaseMutex();
            }
        }

        public Task SaveAsync(string key, IEnumerable<TValue> value)
        {
            var fileName = Path.Combine(this.folderName, $"{key}.json");
            try
            {
                this.@lock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }

            try
            {
                using (var textReader = new StreamWriter(fileName))
                {
                    new JsonSerializer().Serialize(new JsonTextWriter(textReader), value);
                }
            }
            finally
            {
                this.@lock.ReleaseMutex();
            }

            return Task.FromResult(0);
        }

        public void Dispose()
        {
            this.@lock.Dispose();
        }
    }
}
