using DeliverCom.Domain.Delivery;

namespace DeliverCom.API.Model.CreateDelivery
{
    internal class CreateDeliveryResponseDto
    {
        public string CompanyId { get; set; }
        public DeliveryStatus Status { get; set; }
        public string DeliveryId { get; set; }
        public string DeliveryNumber { get; set; }
    }
}