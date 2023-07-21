namespace DeliverCom.Core.Data.Infrastructure.HashStore
{
    public interface IHashStore : IDisposable
    {
        Task<long> HashIncrementAsync(string key, string hashField, long value);
        Task<long> HashIncrementAsync(string key, string hashField, int value);
        Task<double> HashIncrementAsync(string key, string hashField, double value);
        Task<long> HashDecrementAsync(string key, string hashField, long value);
        Task<long> HashDecrementAsync(string key, string hashField, int value);
        Task<double> HashDecrementAsync(string key, string hashField, double value);
        Task<T> HashGetAllAsync<T>(string key) where T : class;

        Task HashSetAsync(string key, IDictionary<string, object> dictionary);

        Task HashSetAsync(string key, IDictionary<string, string> dictionary);
        Task<bool> HashDeleteAsync(string key, string hashField);
        Task<T> HashGetAsync<T>(string path, string key);


        long HashIncrement(string key, string hashField, long value);
        long HashIncrement(string key, string hashField, int value);
        double HashIncrement(string key, string hashField, double value);
        long HashDecrement(string key, string hashField, long value);
        long HashDecrement(string key, string hashField, int value);
        double HashDecrement(string key, string hashField, double value);
        T HashGetAll<T>(string key) where T : class;

        void HashSet(string key, IDictionary<string, object> dictionary);

        void HashSet(string key, IDictionary<string, string> dictionary);
        bool HashDelete(string key, string hashField);
        T HashGet<T>(string path, string key);
    }
}