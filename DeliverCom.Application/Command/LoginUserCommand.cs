using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.UseCase.LoginUser;
using MediatR;

namespace DeliverCom.Application.Command
{
    public class LoginUserCommand : BaseRequest, IRequest<OperationResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public static implicit operator LoginUserUseCaseParameters(LoginUserCommand command)
        {
            return new LoginUserUseCaseParameters
            {
                Email = command.Email,
                Password = command.Password
            };
        }
    }
}