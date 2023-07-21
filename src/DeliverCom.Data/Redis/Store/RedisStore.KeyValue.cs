using DeliverCom.Core.Data.Infrastructure.KeyValueStore;

namespace DeliverCom.Data.Redis.Store
{
    public partial class RedisStore : IKeyValueStore
    {
        public bool Delete(string key)
        {
            var db = GetDatabase();
            var result = db.KeyDelete(key);
            return result;
        }

        public bool Exists(string key)
        {
            var db = GetDatabase();
            var result = db.KeyExists(key);
            return result;
        }

        public bool Expire(string key, TimeSpan expiry)
        {
            var db = GetDatabase();
            var result = db.KeyExpire(key, expiry);
            return result;
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            var db = GetDatabase();
            var redisValue = db.StringGet(key);
            var value = CastValue<T>(redisValue);
            return value;
        }

        public bool Persist(string key)
        {
            var db = GetDatabase();
            var result = db.KeyPersist(key);
            return result;
        }

        public bool Set(string key, object value, TimeSpan? expiry = null)
        {
            var redisValue = GetRedisValue(value);
            var db = GetDatabase();
            var result = db.StringSet(key, redisValue, expiry);
            return result;
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default)
        {
            var db = await GetDatabaseAsync();


            var redisValue = await db.StringGetAsync(key);
            var value = CastValue<T>(redisValue);
            return value;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var db = await GetDatabaseAsync();

            var result = await db.KeyDeleteAsync(key);

            return result;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var db = await GetDatabaseAsync();

            var result = await db.KeyExistsAsync(key);
            return result;
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
        {
            var db = await GetDatabaseAsync();

            var result = await db.KeyExpireAsync(key, expiry);
            return result;
        }

        public async Task<bool> PersistAsync(string key)
        {
            var db = await GetDatabaseAsync();
            var result = await db.KeyPersistAsync(key);
            return result;
        }

        public async Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null)
        {
            var redisValue = GetRedisValue(value);

            var db = await GetDatabaseAsync();

            var result = await db.StringSetAsync(key, redisValue, expiry);

            return result;
        }
    }
}