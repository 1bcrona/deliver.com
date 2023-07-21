namespace DeliverCom.Core.Query.Infrastructure
{
    public interface IQueryBuilder<out TQuery, in TQueryContext> where TQueryContext : IQueryContext
    {
        void BuildFromContext(TQueryContext context);
        TQuery GetQuery();
    }
}