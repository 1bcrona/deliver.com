namespace DeliverCom.UseCase.CreateDelivery
{
    public class CreateDeliveryUseCaseParameters
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
    }
}