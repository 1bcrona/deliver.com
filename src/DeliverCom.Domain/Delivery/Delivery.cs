using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Base;
using DeliverCom.Domain.Delivery.ValueObject;

namespace DeliverCom.Domain.Delivery
{
    public class Delivery : BaseEntity<string>
    {
        private Address _senderAddress;
        private Address _deliveryAddress;
        public Address SenderAddress => _senderAddress;
        public Address DeliveryAddress => _deliveryAddress;
        public virtual Company.Company Company { get; protected set; }

        public DeliveryStatus DeliveryStatus { get; protected set; }
        public string DeliveryNumber { get; protected set; }

        public override void Validate()
        {
            if (SenderAddress == null) throw new ArgumentNullException($"{nameof(SenderAddress)} is null");

            if (DeliveryAddress == null) throw new ArgumentNullException($"{nameof(DeliveryAddress)} is null");

            if (Company == null) throw new ArgumentNullException($"{nameof(Company)} is null");

            if (string.IsNullOrWhiteSpace(SenderAddress.City))
                throw new ArgumentNotValidException($"{nameof(SenderAddress.City)} is not valid");

            if (string.IsNullOrWhiteSpace(SenderAddress.Street))
                throw new ArgumentNotValidException($"{nameof(SenderAddress.Street)} is not valid");

            if (string.IsNullOrWhiteSpace(SenderAddress.Town))
                throw new ArgumentNotValidException($"{nameof(SenderAddress.Town)} is not valid");

            if (string.IsNullOrWhiteSpace(SenderAddress.ZipCode))
                throw new ArgumentNotValidException($"{nameof(SenderAddress.ZipCode)} is not valid");

            if (string.IsNullOrWhiteSpace(DeliveryAddress.City))
                throw new ArgumentNotValidException($"{nameof(DeliveryAddress.City)} is not valid");

            if (string.IsNullOrWhiteSpace(DeliveryAddress.Street))
                throw new ArgumentNotValidException($"{nameof(DeliveryAddress.Street)} is not valid");

            if (string.IsNullOrWhiteSpace(DeliveryAddress.Town))
                throw new ArgumentNotValidException($"{nameof(DeliveryAddress.Town)} is not valid");

            if (string.IsNullOrWhiteSpace(DeliveryAddress.ZipCode))
                throw new ArgumentNotValidException($"{nameof(DeliveryAddress.ZipCode)} is not valid");
        }

        protected override void SetId()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        private static Address newAddress(string street, string town, string city, string zipCode)
        {
            return new Address(street, town, city, zipCode);
        }


        public void SetSenderAddress(string street, string town, string city, string zipCode)
        {
            _senderAddress = newAddress(street, town, city, zipCode);
        }

        public void SetDeliveryAddress(string street, string town, string city, string zipCode)
        {
            _deliveryAddress = newAddress(street, town, city, zipCode);
        }


        public static Delivery NewDelivery(Address senderAddress, Address deliveryAddress, Company.Company company)
        {
            if (senderAddress == null) throw new ArgumentNullException($"{nameof(senderAddress)} is null");
            if (deliveryAddress == null) throw new ArgumentNullException($"{nameof(deliveryAddress)} is null");
            if (company == null) throw new ArgumentNullException($"{nameof(company)} is null");

            var delivery = new Delivery
            {
                Company = company,
                DeliveryStatus = DeliveryStatus.NotShipped
            };
            delivery.SetSenderAddress(senderAddress.Street, senderAddress.Town, senderAddress.City,
                senderAddress.ZipCode);
            delivery.SetDeliveryAddress(deliveryAddress.Street, deliveryAddress.Town, deliveryAddress.City,
                deliveryAddress.ZipCode);
            delivery.SetDeliveryNumber();
            delivery.SetEntityId();
            delivery.SetId();
            delivery.Activate();
            delivery.EnsureCreationDate();
            delivery.EnsureUpdateDate();
            delivery.Validate();
            return delivery;
        }

        private void SetDeliveryNumber()
        {
            DeliveryNumber = Guid.NewGuid().ToString("D").ToLowerInvariant();
        }
    }
}