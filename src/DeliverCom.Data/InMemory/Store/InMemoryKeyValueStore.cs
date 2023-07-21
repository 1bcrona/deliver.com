#region

using System.Collections.Concurrent;
using DeliverCom.Core.Data.Infrastructure.KeyValueStore;
using Newtonsoft.Json;

#endregion

namespace DeliverCom.Data.InMemory.Store
{
    public class InMemoryKeyValueStore : IKeyValueStore
    {
        public InMemoryKeyValueStore()
        {
            Name = _defaultKeyValueStoreName;
        }

        public string Name { get; }

        ~InMemoryKeyValueStore()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            var store = Interlocked.Exchange(ref _store, null);
            store?.Clear();
        }

        private const string _defaultKeyValueStoreName = "__kv_default__";
        private ConcurrentDictionary<string, KeyValueStoreEntry> _store = new();

        public bool Delete(string key)
        {
            return _store.TryRemove(key, out _);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await Task.FromResult(Delete(key));
        }

        public bool Exists(string key)
        {
            return GetEntry(key) != null;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await Task.FromResult(Exists(key));
        }

        public bool Expire(string key, TimeSpan expiry)
        {
            var entry = GetEntry(key);

            if (entry == null) return false;

            entry.Key = key;
            entry.ExpireDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)expiry.TotalSeconds;
            _store[key] = entry;
            return true;
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
        {
   
            return await Task.FromResult(         Expire(key, expiry));
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            var entry = GetEntry(key);
            return CastValue(entry, defaultValue);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default)
        {
            return await Task.FromResult(Get(key, defaultValue));
        }

        public bool Persist(string key)
        {
            var entry = GetEntry(key, false);

            if (entry == null) return false;

            entry.ExpireDate = DateTimeOffset.MaxValue.ToUnixTimeSeconds();
            _store[key] = entry;

            return true;
        }

        public async Task<bool> PersistAsync(string key)
        {
            return await Task.FromResult(  Persist(key));
        }

        public bool Set(string key, object value, TimeSpan? expiry = null)
        {
            if (value == null) return false;
            var entry = GetEntry(key);

            var creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            entry ??= new KeyValueStoreEntry();
            entry.Key = key;
            entry.Item = value;
            entry.CreationDate = creationDate;
            entry.ExpireDate = expiry.HasValue
                ? creationDate + (long)expiry.Value.TotalSeconds
                : DateTimeOffset.MaxValue.ToUnixTimeSeconds();

            _store.AddOrUpdate(key, entry, (_, _) => entry);

            return true;
        }

        public async Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null)
        {
            return await Task.FromResult(Set(key, value, expiry));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task Flush()
        {
            _store.Clear();
            return Task.FromResult(true);
        }

        private T CastValue<T>(KeyValueStoreEntry entry, T defaultValue = default)
        {
            return entry == null ? defaultValue : CastValue(entry.Key, defaultValue);
        }

        private T CastValue<T>(string key, T defaultValue = default)
        {
            var type = typeof(T);
            if (!_store.TryGetValue(key, out var existingEntry)) return defaultValue;
            var value = existingEntry.Item;

            try
            {
                if (type.IsSerializable) return (T)Convert.ChangeType(value, type);

                var js = JsonConvert.SerializeObject(value);
                return JsonConvert.DeserializeObject<T>(js);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private KeyValueStoreEntry GetEntry(string key, bool deleteIfExpire = true)
        {
            KeyValueStoreEntry rv = null;
            try
            {
                if (_store.TryGetValue(key, out rv))
                {
                    if (!deleteIfExpire) return rv;
                    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= rv.ExpireDate) return rv;
                    _store.TryRemove(key, out rv);
                    return null;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return rv;
        }
    }
}