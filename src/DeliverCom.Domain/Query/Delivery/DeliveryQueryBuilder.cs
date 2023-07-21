using System.Linq.Expressions;
using DeliverCom.Core.Query.Infrastructure;
using DeliverCom.Core.Query.Infrastructure.Base;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Query.Impl;

namespace DeliverCom.Domain.Query.Delivery
{
    public class DeliveryQueryBuilder : IQueryBuilder<QueryBase<Expression<Func<Domain.Delivery.Delivery, bool>>>,
        DeliveryQueryContext>
    {
        private readonly QueryBase<Expression<Func<Domain.Delivery.Delivery, bool>>> _queryBase;

        public DeliveryQueryBuilder()
        {
            _queryBase = new LinqQuery<Domain.Delivery.Delivery>();
        }

        public void BuildFromContext(DeliveryQueryContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.CompanyId))
                _queryBase.Append(x => x.Company.EntityId == context.CompanyId);

            if (!string.IsNullOrWhiteSpace(context.SenderCity))
                _queryBase.Append(x => x.SenderAddress.City == context.SenderCity);

            if (!string.IsNullOrWhiteSpace(context.SenderTown))
                _queryBase.Append(x => x.SenderAddress.Town == context.SenderTown);

            if (!string.IsNullOrWhiteSpace(context.DeliveryCity))
                _queryBase.Append(x => x.DeliveryAddress.City == context.DeliveryCity);

            if (!string.IsNullOrWhiteSpace(context.DeliveryTown))
                _queryBase.Append(x => x.DeliveryAddress.Town == context.DeliveryTown);

            if (context.DeliveryStatus != DeliveryStatus.None)
                _queryBase.Append(x => x.DeliveryStatus == context.DeliveryStatus);

            if (!string.IsNullOrWhiteSpace(context.DeliveryId))
                _queryBase.Append(x => x.EntityId == context.DeliveryId);

            if (!string.IsNullOrWhiteSpace(context.DeliveryNumber))
                _queryBase.Append(x => x.DeliveryNumber == context.DeliveryNumber);
        }

        public QueryBase<Expression<Func<Domain.Delivery.Delivery, bool>>> GetQuery()
        {
            return _queryBase;
        }
    }
}