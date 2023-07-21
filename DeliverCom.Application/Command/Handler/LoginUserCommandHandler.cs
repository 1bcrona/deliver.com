using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.UseCase.LoginUser;

namespace DeliverCom.Application.Command.Handler
{
    public class LoginUserCommandHandler : BaseRequestHandler<LoginUserCommand, OperationResult>
    {
        public LoginUserCommandHandler(IRouter router) : base(router)
        {
        }

        public override async Task<OperationResult> Handle(LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var useCase = _router.Route("LoginUserUseCase", (LoginUserUseCaseParameters)request);
            return await useCase.Execute();
        }
    }
}