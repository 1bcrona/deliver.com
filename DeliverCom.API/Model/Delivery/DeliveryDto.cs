using DeliverCom.API.Model.Address;
using DeliverCom.API.Model.Company;
using DeliverCom.Domain.Delivery;

namespace DeliverCom.API.Model.Delivery
{
    public class DeliveryDto
    {
        public CompanyDto Company { get; set; }
        public AddressDto SenderAddress { get; set; }
        public AddressDto DeliveryAddress { get; set; }
        public DeliveryStatus Status { get; set; }
        public string DeliveryId { get; set; }
        public string DeliveryNumber { get; set; }
    }
}