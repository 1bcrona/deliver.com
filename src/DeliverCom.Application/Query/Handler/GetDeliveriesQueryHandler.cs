using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Domain.Query;
using DeliverCom.Domain.Query.Delivery;

namespace DeliverCom.Application.Query.Handler
{
    public class GetDeliveriesQueryHandler : BaseRequestHandler<GetDeliveriesQuery, OperationResult>
    {
        private readonly IExecutionContext _executionContext;

        public GetDeliveriesQueryHandler(IRouter router, IExecutionContext executionContext) : base(router)
        {
            _executionContext = executionContext;
        }

        public override async Task<OperationResult> Handle(GetDeliveriesQuery request,
            CancellationToken cancellationToken)
        {
            var queryContext = new DeliveryQueryContext
            {
                CompanyId = request.CompanyId ?? _executionContext.Identity.CompanyId,
                SenderCity = request.SenderCity,
                SenderTown = request.SenderTown,
                DeliveryCity = request.DeliveryCity,
                DeliveryTown = request.DeliveryTown,
                DeliveryStatus = request.DeliveryStatus,
                DeliveryId = request.DeliveryId,
                DeliveryNumber = request.DeliveryNumber,
                PaginationContext = new PaginationContext
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                }
            };

            var useCase = _router.Route("GetDeliveriesUseCase", queryContext);

            return await useCase.Execute();
        }
    }
}