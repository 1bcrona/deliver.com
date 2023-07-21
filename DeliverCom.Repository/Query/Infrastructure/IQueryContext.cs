namespace DeliverCom.Repository.Query.Infrastructure
{
    public interface IQueryContext
    {
        void FillQueryContext(Dictionary<string, string> args);
    }
}