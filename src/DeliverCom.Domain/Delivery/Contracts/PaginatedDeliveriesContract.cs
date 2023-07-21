namespace DeliverCom.Domain.Delivery.Contracts
{
    public class PaginatedDeliveriesContract
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Delivery[] Deliveries { get; set; }
    }
}