using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Domain.Context;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Delivery.Contracts;
using DeliverCom.Domain.Query.Delivery;

namespace DeliverCom.UseCase.GetDeliveries
{
    public class GetDeliveriesUseCase : BaseUseCase<DeliveryQueryContext>
    {
        private readonly DeliverComDbContext _deliverComDbContext;

        public GetDeliveriesUseCase(DeliverComDbContext deliverComDbContext)
        {
            _deliverComDbContext = deliverComDbContext;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNullException(nameof(Input));

            if (string.IsNullOrWhiteSpace(Input.CompanyId))
                throw new ArgumentNotValidException(nameof(Input.CompanyId));
        }

        protected override async Task<OperationResult> ExecuteInternal()
        {
            if (AmbientExecutionContext == null)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.Identity == null)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.IsEmpty)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.Identity.CompanyId != Input.CompanyId)
                throw new AuthenticationException("Authentication failed");
            var deliveryQueryDirector = new DeliverQueryDirector();
            var deliveryQueryBuilder = new DeliveryQueryBuilder();
            deliveryQueryDirector.SetQueryBuilder(deliveryQueryBuilder);
            deliveryQueryDirector.Construct(Input);
            var rawQuery = deliveryQueryDirector.GetRawQuery();
            var deliveries = _deliverComDbContext.Deliveries.Where(rawQuery);
            var contract = new PaginatedDeliveriesContract();
            var items = Array.Empty<Delivery>();
            if (Input.PaginationContext != null)
                if (Input.PaginationContext.PageSize > 0 && Input.PaginationContext.PageNumber > 0)
                {
                    items = deliveries
                        .Skip(Input.PaginationContext.PageSize * (Input.PaginationContext.PageNumber - 1))
                        .Take(Input.PaginationContext.PageSize).ToArray();
                    contract.PageSize = Input.PaginationContext.PageSize;
                    contract.PageNumber = Input.PaginationContext.PageNumber;
                    contract.Deliveries = items;
                    contract.TotalCount = deliveries.Count();
                    return await Task.FromResult(OperationResult.Ok(contract));
                }

            items = deliveries.ToArray();
            contract.Deliveries = items;
            contract.TotalCount = items.Length;
            contract.PageSize = items.Length;
            contract.PageNumber = 1;
            return await Task.FromResult(OperationResult.Ok(contract));
        }
    }
}