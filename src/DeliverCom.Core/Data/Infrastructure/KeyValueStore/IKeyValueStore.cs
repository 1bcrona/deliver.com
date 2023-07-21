namespace DeliverCom.Core.Data.Infrastructure.KeyValueStore
{
    public interface IKeyValueStore : IDisposable
    {
        Task<bool> DeleteAsync(string key);

        Task<bool> ExistsAsync(string key);

        Task<bool> ExpireAsync(string key, TimeSpan expiry);


        Task<T> GetAsync<T>(string key, T defaultValue = default);

        Task<bool> PersistAsync(string key);

        Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null);


        bool Delete(string key);

        bool Exists(string key);

        bool Expire(string key, TimeSpan expiry);


        T Get<T>(string key, T defaultValue = default);

        bool Persist(string key);

        bool Set(string key, object value, TimeSpan? expiry = null);
    }
}