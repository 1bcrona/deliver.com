using DeliverCom.Application.Infrastructure;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Domain.Delivery;
using MediatR;

namespace DeliverCom.Application.Query
{
    public class GetDeliveriesQuery : BaseRequest, IRequest<OperationResult>
    {
        public string CompanyId { get; set; }
        public string SenderCity { get; set; }
        public string SenderTown { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryTown { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryId { get; set; }
        public string DeliveryNumber { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }


    /* Create Handler  For GetDeliveriesQuery */
}