using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.UseCase.RegisterUser;

namespace DeliverCom.Application.Command.Handler
{
    public class RegisterUserCommandHandler : BaseRequestHandler<RegisterUserCommand, OperationResult>
    {
        public RegisterUserCommandHandler(IRouter router) : base(router)
        {
        }

        public override async Task<OperationResult> Handle(RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var useCase = _router.Route("RegisterUserUseCase", (RegisterUserUseCaseParameters)request);
            return await useCase.Execute();
        }
    }
}