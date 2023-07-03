using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChainResource
{
    public class ChainResource<T>
    {
        private Storage<T>[] storages;

        public ChainResource(params Storage<T>[] storages)
        {
            this.storages = storages;
        }

        public async Task<T> GetValue()
        {
            foreach (Storage<T> storage in storages)
            {
                var value = await storage.Read();
                if (value != null)
                {
                    await PropagateValue(value);
                    return value;
                }
            }

            return default(T);
        }

        private async Task PropagateValue(T value)
        {
            foreach (var storage in storages)
            {
                if (!storage.ReadOnly)
                {
                    await storage.Write(value);
                }
            }
        }
    }

    public abstract class Storage<T>
    {
        public bool ReadOnly { get; }
        public TimeSpan Expiration { get; }

        protected Storage(bool readOnly, TimeSpan expiration)
        {
            ReadOnly = readOnly;
            Expiration = expiration;
        }

        public abstract Task<T> Read();
        public abstract Task Write(T value);
    }

    public class MemoryStorage<T> : Storage<T>
    {
        private T data;
        private DateTime lastUpdated;

        public MemoryStorage(TimeSpan expiration)
            : base(false, expiration)
        {
        }

        public override Task<T> Read()
        {
            if (IsExpired())
            {
                return Task.FromResult<T>(default);
            }
            return Task.FromResult(data);
        }

        public override Task Write(T value)
        {
            data = value;
            lastUpdated = DateTime.Now;
            return Task.CompletedTask;
        }

        private bool IsExpired()
        {
            return DateTime.Now - lastUpdated > Expiration;
        }
    }

    public class FileSystemStorage<T> : Storage<T>
    {
        private string filePath;

        public FileSystemStorage(TimeSpan expiration, string filePath)
            : base(false, expiration)
        {
            this.filePath = filePath;
        }

        public override Task<T> Read()
        {
            if (IsExpired())
            {
                return Task.FromResult<T>(default);
            }

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return Task.FromResult(JsonSerializer.Deserialize<T>(json));
            }

            return Task.FromResult<T>(default);
        }

        public override Task Write(T value)
        {
            var json = JsonSerializer.Serialize(value);
            File.WriteAllText(filePath, json);
            return Task.CompletedTask;
        }

        private bool IsExpired()
        {
            return DateTime.Now - File.GetLastWriteTime(filePath) > Expiration;
        }
    }

    public class WebServiceStorage<T> : Storage<T>
    {
        private string apiUrl;
        private HttpClient httpClient;

        public WebServiceStorage(string apiUrl)
            : base(true, TimeSpan.Zero)
        {
            this.apiUrl = apiUrl;
            httpClient = new HttpClient();
        }

        public override async Task<T> Read()
        {
            var response = await httpClient.GetAsync(apiUrl + "?app_id=bc7d7c0c29f9426c8b88805ed4b33610");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json);
            }

            return default(T);
        }

        public override Task Write(T value)
        {
            throw new Exception("WebService does not have a  write operation.");
        }
    }
}
