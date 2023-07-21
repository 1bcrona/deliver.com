using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.UseCase.CreateDelivery;

namespace DeliverCom.Application.Command.Handler
{
    public class CreateDeliveryCommandHandler : BaseRequestHandler<CreateDeliveryCommand, OperationResult>
    {
        private readonly IExecutionContext _context;

        public CreateDeliveryCommandHandler(IRouter router, IExecutionContext context) : base(router)
        {
            _context = context;
        }

        public override async Task<OperationResult> Handle(CreateDeliveryCommand request,
            CancellationToken cancellationToken)
        {
            var useCase = _router.Route("CreateDeliveryUseCase", (CreateDeliveryUseCaseParameters)request);
            return await useCase.Execute();
        }
    }
}