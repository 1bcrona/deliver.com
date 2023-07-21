namespace DeliverCom.Core.Query.Infrastructure.Base
{
    public abstract class QueryBase<TQuery> : IQuery<TQuery>
    {
        protected TQuery RawQuery { get; set; }
        public abstract TQuery GetRawQuery();
        public abstract void Append(TQuery query);
    }
}