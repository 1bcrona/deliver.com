#region

using System.Text;
using DeliverCom.Core.Helper;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = DeliverCom.Core.Serialization.JsonSerializer;

#endregion

namespace DeliverCom.Data.Redis.Store
{
    public partial class RedisStore
    {
        private readonly string _connectionString;
        private readonly int _db;
        private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private static ConnectionMultiplexer _connection;
        private static readonly object _connectionLock = new();
        private const int _maxValueLength = 100 * 1024; //100KB for Redis

        private static JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.All, NullValueHandling = NullValueHandling.Ignore,
        };

        public RedisStore(string connectionString, int db = 0)
        {
            _connectionString = connectionString;
            _db = db;
        }

        private bool Open => _connection is { IsConnected: true };


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IDatabase GetDatabase()
        {
            Connect();
            return _connection.GetDatabase(_db);
        }

        private async Task<IDatabase> GetDatabaseAsync()
        {
            return await Task.FromResult(GetDatabase());
        }

        private void Connect()
        {
            if (Open) return;
            lock (_connectionLock)
            {
                if (Open) return;
                var conn = Interlocked.Exchange(ref _connection, null);

                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }

                var configurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    EndPoints = { _connectionString }
                };
                conn = ConnectionMultiplexer.Connect(configurationOptions);

                if (!conn.IsConnected)
                {
                    throw new RedisException(
                        $"Cannot create redis connection : '{_connectionString}:{_db}'");
                }

                Interlocked.Exchange(ref _connection, conn);
            }
        }

        private RedisValue GetRedisValue(object value)
        {
            if (value == null) return RedisValue.Null;


            var type = value.GetType();
            var isPrimitiveType = (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);

            var valueString = isPrimitiveType
                ? value.ToString()
                : JsonSerializer.Serialize(value, _settings);

            if (valueString.Length <= _maxValueLength) return valueString;

            var bytes = Encoding.UTF8.GetBytes(valueString);
            var compressed = ZipHelper.Compress(bytes);
            return compressed;
        }

        private static T CastValue<T>(RedisValue redisValue, T defaultValue = default)
        {
            var type = typeof(T);

            if (!redisValue.HasValue)
                return defaultValue;

            if (redisValue.IsNull)
                return default;

            var value = String.Empty;
            var bytes = (byte[])redisValue;
            value = Encoding.UTF8.GetString(ZipHelper.IsGZipped(bytes) ? ZipHelper.Decompress(bytes) : bytes);

            var isPrimitiveType = (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
            if (!isPrimitiveType)
                return JsonSerializer.Deserialize<T>(value);

            try
            {
                type = Nullable.GetUnderlyingType(type) ?? type;

                if (type.IsEnum)
                {
                    return (T)Enum.Parse(type, value);
                }

                return (T)Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return default;
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            var conn = Interlocked.Exchange(ref _connection, null);
            conn?.Close();
            conn?.Dispose();
            _semaphoreSlim.Dispose();
        }

        ~RedisStore()
        {
            Dispose(false);
        }
    }
}