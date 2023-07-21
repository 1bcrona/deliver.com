using System.Linq.Expressions;
using DeliverCom.Core.Query.Infrastructure;

namespace DeliverCom.Domain.Query.Delivery
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