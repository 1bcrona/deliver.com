using System.Linq.Expressions;
using DeliverCom.Repository.Query.Infrastructure;

namespace DeliverCom.Repository.Query.Delivery
{
    public class DeliverQueryDirector : IQueryDirector<DeliveryQueryBuilder, DeliveryQueryContext>
    {
        private DeliveryQueryBuilder _builder;

        public void Construct(DeliveryQueryContext context)
        {
            _builder.BuildFromContext(context);
        }

        public void SetQueryBuilder(DeliveryQueryBuilder queryBuilder)
        {
            _builder = queryBuilder;
        }

        public Expression<Func<Domain.Delivery.Delivery, bool>> GetRawQuery()
        {
            return _builder.GetQuery().GetRawQuery();
        }
    }
}