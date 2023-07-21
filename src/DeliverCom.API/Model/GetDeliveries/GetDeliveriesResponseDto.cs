using DeliverCom.API.Model.Delivery;

namespace DeliverCom.API.Model.GetDeliveries
{
    public class GetDeliveriesResponseDto
    {
        public long TotalCount { get; internal set; }
        public DeliveryDto[] Deliveries { get; internal set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}