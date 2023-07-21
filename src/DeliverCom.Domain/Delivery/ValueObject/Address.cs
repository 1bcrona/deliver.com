namespace DeliverCom.Domain.Delivery.ValueObject
{
    public class Address
    {
        private string _street;
        private string _town;
        private string _city;
        private string _zipCode;

        public Address(string street, string town, string city, string zipCode)
        {
            _street = street;
            _town = town;
            _city = city;
            _zipCode = zipCode;
        }

        public string Street => _street;

        public string Town => _town;

        public string City => _city;

        public string ZipCode => _zipCode;
    }
}