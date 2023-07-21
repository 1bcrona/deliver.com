using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.UseCase.CreateDelivery;
using MediatR;

namespace DeliverCom.Application.Command
{
    public class CreateDeliveryCommand : BaseRequest, IRequest<OperationResult>
    {
        public string CompanyId { get; set; }
        public string SenderAddressCity { get; set; }
        public string SenderAddressStreet { get; set; }
        public string SenderAddressTown { get; set; }
        public string SenderAddressZipCode { get; set; }
        public string DeliveryAddressCity { get; set; }
        public string DeliveryAddressStreet { get; set; }
        public string DeliveryAddressTown { get; set; }
        public string DeliveryAddressZipCode { get; set; }


        public static implicit operator CreateDeliveryUseCaseParameters(CreateDeliveryCommand command)
        {
            return new CreateDeliveryUseCaseParameters
            {
                CompanyId = command.CompanyId,
                SenderAddressCity = command.SenderAddressCity,
                SenderAddressStreet = command.SenderAddressStreet,
                SenderAddressTown = command.SenderAddressTown,
                SenderAddressZipCode = command.SenderAddressZipCode,
                DeliveryAddressCity = command.DeliveryAddressCity,
                DeliveryAddressStreet = command.DeliveryAddressStreet,
                DeliveryAddressTown = command.DeliveryAddressTown,
                DeliveryAddressZipCode = command.DeliveryAddressZipCode
            };
        }
    }
}