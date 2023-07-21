using DeliverCom.Domain.Delivery;

namespace DeliverCom.API.Model.GetDeliveries
{
    public class GetDeliveriesRequestDto
    {
        public string SenderCity { get; set; }
        public string SenderTown { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryTown { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryId { get; set; }
        public string DeliveryNumber { get; set; }
    }
}