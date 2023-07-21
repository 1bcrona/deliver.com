using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.UseCase.RegisterCompany;
using MediatR;

namespace DeliverCom.Application.Command
{
    public class RegisterCompanyCommand : BaseRequest, IRequest<OperationResult>
    {
        public string Name { get; set; }

        public static implicit operator RegisterCompanyUseCaseParameters(RegisterCompanyCommand command)
        {
            return new RegisterCompanyUseCaseParameters
            {
                Name = command.Name
            };
        }
    }
}