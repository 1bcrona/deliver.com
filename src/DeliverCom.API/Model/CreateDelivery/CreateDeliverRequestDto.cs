using DeliverCom.API.Model.Address;

namespace DeliverCom.API.Model.CreateDelivery
{
    public class CreateDeliverRequestDto
    {
        public string CompanyId { get; set; }
        public AddressDto SenderAddress { get; set; }
        public AddressDto DeliveryAddress { get; set; }
    }
}