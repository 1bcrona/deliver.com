using DeliverCom.Core.Data.Infrastructure.HashStore;
using DeliverCom.Core.Serialization;
using StackExchange.Redis;

namespace DeliverCom.Data.Redis.Store
{
    public partial class RedisStore : IHashStore
    {
        public async Task<long> HashIncrementAsync(string key, string hashField, long value)
        {
            var db = await GetDatabaseAsync();
            return await db.HashIncrementAsync(key, hashField, value);
        }

        public async Task<long> HashIncrementAsync(string key, string hashField, int value)
        {
            var db = await GetDatabaseAsync();
            return await db.HashIncrementAsync(key, hashField, value);
        }

        public async Task<double> HashIncrementAsync(string key, string hashField, double value)
        {
            var db = await GetDatabaseAsync();
            return await db.HashIncrementAsync(key, hashField, value);
        }

        public async Task<long> HashDecrementAsync(string key, string hashField, long value)
        {
            var db = await GetDatabaseAsync();

            return await db.HashDecrementAsync(key, hashField, value);
        }

        public async Task<long> HashDecrementAsync(string key, string hashField, int value)
        {
            var db = await GetDatabaseAsync();

            return await db.HashDecrementAsync(key, hashField, value);
        }

        public async Task<double> HashDecrementAsync(string key, string hashField, double value)
        {
            var db = await GetDatabaseAsync();
            return await db.HashDecrementAsync(key, hashField, value);
        }

        public async Task<T> HashGetAllAsync<T>(string key) where T : class
        {
            var db = await GetDatabaseAsync();

            var hashEntries = await db.HashGetAllAsync(key);

            return HashGetAll<T>(hashEntries);
        }


        public async Task<T> HashGetAsync<T>(string path, string key)
        {
            var db = await GetDatabaseAsync();

            var entryDictionary = (await db.HashGetAllAsync(path)).ToDictionary(
                x => x.Name.ToString(),
                x => x.Value,
                StringComparer.Ordinal);

            var entry = entryDictionary.FirstOrDefault(k => k.Key == key);
            return entry.Value.IsNullOrEmpty ? default : CastHashValue<T>(entry.Value);
        }


        public async Task<bool> HashDeleteAsync(string key, string hashField)
        {
            var db = await GetDatabaseAsync();
            return await db.HashDeleteAsync(key, hashField);
        }

        public async Task HashSetAsync(string key, IDictionary<string, string> dictionary)
        {
            var db = await GetDatabaseAsync();

            var hashEntries = dictionary.Select(
                pair => new HashEntry(pair.Key, GetRedisHashValue(pair.Value))).ToArray();

            await db.HashSetAsync(key, hashEntries);
        }

        public async Task HashSetAsync(string key, IDictionary<string, object> dictionary)
        {
            var db = await GetDatabaseAsync();

            var hashEntries = dictionary.Select(
                pair => new HashEntry(pair.Key, GetRedisHashValue(pair.Value))).ToArray();

            await db.HashSetAsync(key, hashEntries);
        }


        public long HashIncrement(string key, string hashField, long value)
        {
            var db = GetDatabase();
            return db.HashIncrement(key, hashField, value);
        }

        public long HashIncrement(string key, string hashField, int value)
        {
            var db = GetDatabase();
            return db.HashIncrement(key, hashField, value);
        }

        public double HashIncrement(string key, string hashField, double value)
        {
            var db = GetDatabase();
            return db.HashIncrement(key, hashField, value);
        }

        public long HashDecrement(string key, string hashField, long value)
        {
            var db = GetDatabase();

            return db.HashDecrement(key, hashField, value);
        }

        public long HashDecrement(string key, string hashField, int value)
        {
            var db = GetDatabase();

            return db.HashDecrement(key, hashField, value);
        }

        public double HashDecrement(string key, string hashField, double value)
        {
            var db = GetDatabase();
            return db.HashDecrement(key, hashField, value);
        }

        public T HashGetAll<T>(string key) where T : class
        {
            var db = GetDatabase();

            var hashEntries = db.HashGetAll(key);

            return HashGetAll<T>(hashEntries);
        }


        public T HashGet<T>(string path, string key)
        {
            var db = GetDatabase();

            var entryDictionary = db.HashGetAll(path).ToDictionary(
                x => x.Name.ToString(),
                x => x.Value,
                StringComparer.Ordinal);

            var entry = entryDictionary.FirstOrDefault(k => k.Key == key);
            if (entry.Value.IsNullOrEmpty)
                return default;


            return CastHashValue<T>(entry.Value);
        }


        public bool HashDelete(string key, string hashField)
        {
            var db = GetDatabase();
            return db.HashDelete(key, hashField);
        }

        public void HashSet(string key, IDictionary<string, string> dictionary)
        {
            var db = GetDatabase();

            var hashEntries = dictionary.Select(
                pair => new HashEntry(pair.Key, GetRedisHashValue(pair.Value))).ToArray();

            db.HashSet(key, hashEntries);
        }

        public void HashSet(string key, IDictionary<string, object> dictionary)
        {
            var db = GetDatabase();

            var hashEntries = dictionary.Select(
                pair => new HashEntry(pair.Key, GetRedisHashValue(pair.Value))).ToArray();

            db.HashSet(key, hashEntries);
        }


        private T CastHashValue<T>(RedisValue value)
        {
            return (T)CastHashValue(value, typeof(T));
        }

        private object CastHashValue(RedisValue value, Type type)
        {
            return JsonSerializer.Deserialize(value, type);
        }

        private RedisValue GetRedisHashValue(object value)
        {
            if (value == null) return RedisValue.Null;
            RedisValue redisValue = JsonSerializer.Serialize(value);
            return redisValue;
        }


        private T HashGetAll<T>(HashEntry[] hashEntries)
        {
            var instance = Activator.CreateInstance<T>();
            switch (instance)
            {
                case IDictionary<string, object> dictionary:
                {
                    foreach (var entry in hashEntries) dictionary[entry.Name] = CastHashValue<object>(entry.Value);

                    return (T)dictionary;
                }
                case IDictionary<string, string> stringDictionary:
                {
                    foreach (var entry in hashEntries)
                        stringDictionary[entry.Name] = CastHashValue<string>(entry.Value);

                    return (T)stringDictionary;
                }
                default:
                {
                    var props = instance.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        if (!prop.CanWrite) continue;

                        var propName = prop.Name;
                        var entry = hashEntries.FirstOrDefault(w => w.Name == propName);
                        if (entry.Value == RedisValue.Null) continue;
                        var propType = prop.PropertyType;
                        var converted = CastHashValue(entry.Value, propType);
                        prop.SetValue(instance, converted);
                    }

                    return instance;
                }
            }
        }
    }
}