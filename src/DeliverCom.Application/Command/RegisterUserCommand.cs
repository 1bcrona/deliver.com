using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Domain.User.ValueObject;
using DeliverCom.UseCase.RegisterUser;
using MediatR;

namespace DeliverCom.Application.Command
{
    public class RegisterUserCommand : BaseRequest, IRequest<OperationResult>
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public UserRole Role { get; set; }
        public string CompanyId { get; set; }
        public string Password { get; set; }

        public static implicit operator RegisterUserUseCaseParameters(RegisterUserCommand command)
        {
            return new RegisterUserUseCaseParameters
            {
                CompanyId = command.CompanyId,
                EmailAddress = command.EmailAddress,
                FirstName = command.FirstName,
                Role = command.Role,
                Surname = command.Surname,
                Password = command.Password
            };
        }
    }
}