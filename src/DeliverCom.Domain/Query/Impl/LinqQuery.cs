using System.Linq.Expressions;
using DeliverCom.Core.Helper;
using DeliverCom.Core.Query.Infrastructure.Base;
using DeliverCom.Domain.Base;
using LinqKit;

namespace DeliverCom.Domain.Query.Impl
{
    public class LinqQuery<T> : QueryBase<Expression<Func<T, bool>>> where T : BaseEntity<string>
    {
        public LinqQuery()
        {
            RawQuery = x => true;
        }

        public override Expression<Func<T, bool>> GetRawQuery()
        {
            return RawQuery.Expand();
        }

        public override void Append(Expression<Func<T, bool>> query)
        {
            RawQuery = ExpressionHelper.And(RawQuery, query);
        }
    }
}