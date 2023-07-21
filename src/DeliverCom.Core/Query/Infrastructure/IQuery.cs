namespace DeliverCom.Core.Query.Infrastructure
{
    public interface IQuery<T>
    {
        T GetRawQuery();
        void Append(T query);
    }
}