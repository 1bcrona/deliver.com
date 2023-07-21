using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.UseCase.RegisterCompany;

namespace DeliverCom.Application.Command.Handler
{
    public class RegisterCompanyCommandHandler : BaseRequestHandler<RegisterCompanyCommand, OperationResult>
    {
        public RegisterCompanyCommandHandler(IRouter router) : base(router)
        {
        }

        public override async Task<OperationResult> Handle(RegisterCompanyCommand request,
            CancellationToken cancellationToken)
        {
            var useCase = _router.Route("RegisterCompanyUseCase", (RegisterCompanyUseCaseParameters)request);
            return await useCase.Execute();
        }
    }
}