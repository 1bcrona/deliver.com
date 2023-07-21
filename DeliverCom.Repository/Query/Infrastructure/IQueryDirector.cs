namespace DeliverCom.Repository.Query.Infrastructure
{
    public interface IQueryDirector<in T, in TQueryContext>
    {
        void SetQueryBuilder(T queryBuilder);
        void Construct(TQueryContext context);
    }
}